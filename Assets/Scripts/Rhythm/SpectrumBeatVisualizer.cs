using UnityEngine;
using UnityEngine.UI;

public class SpectrumBeatVisualizer : MonoBehaviour
{
    [Header("Audio & Rhythm")]
    public AudioSource source;
    public BeatConductor conductor;
    public BeatMapData map;

    [Header("UI Bars")]
    public RectTransform barsParent;
    public Image barPrefab;
    [Range(8, 128)] public int barCount = 48;
    public float minHeight = 6f;
    public float maxHeight = 120f;

    [Header("Spectrum")]
    [Tooltip("FFT size must be power-of-two [64..8192]")]
    public int fftSize = 1024;
    public FFTWindow fftWindow = FFTWindow.BlackmanHarris;
    [Tooltip("Log scale gives nicer distribution of low/high freqs")]
    public bool useLogFrequencyScale = true;
    [Tooltip("Visual gain multiplier for spectrum amplitudes")]
    public float gain = 35f;
    [Tooltip("0=no smoothing, 1=very slow")]
    [Range(0f, 0.98f)] public float smooth = 0.5f;

    // enum БЕЗ атрибутов
    public enum PerfectCueMode { AllBars, EveryNthBar, ChangeColor }

    [Header("Perfect cue")] // ← атрибут стоит над ПОЛЕМ, а не над enum
    public PerfectCueMode perfectCue = PerfectCueMode.AllBars;

    [Tooltip("For EveryNthBar mode")]
    public int nth = 3;
    [Tooltip("Hold time (sec) to keep the visual cue (e.g. color)")]
    public float perfectHoldSec = 0.04f;
    public Color normalColor = Color.white;
    public Color perfectColor = Color.yellow;

    [Header("Perfect boost (multiplier)")]
    [Tooltip("Multiply bar height on Perfect (applied once on trigger, clamped to maxHeight). 1 = no boost")]
    public float perfectMultiplier = 1.35f;

    [Header("Auto size")]
    public bool fitMaxHeightToParent = true;

    float[] _spectrum;
    Image[] _bars;
    float[] _curHeights;
    float _perfectHoldTimer;
    bool _wasPerfect;
    bool _perfectPulse;

    void Awake()
    {
        if (!source) source = FindObjectOfType<AudioSource>();
        if (!conductor) conductor = FindObjectOfType<BeatConductor>();
        if (!map && conductor) map = conductor.map;
    }

    void Start()
    {
        BuildBars();
        _spectrum = new float[Mathf.ClosestPowerOfTwo(Mathf.Clamp(fftSize, 64, 8192))];
        if (_spectrum.Length != fftSize) fftSize = _spectrum.Length;
    }

    void BuildBars()
    {
        for (int i = barsParent.childCount - 1; i >= 0; i--)
            Destroy(barsParent.GetChild(i).gameObject);

        _bars = new Image[barCount];
        _curHeights = new float[barCount];

        for (int i = 0; i < barCount; i++)
        {
            var img = Instantiate(barPrefab, barsParent);
            img.rectTransform.anchorMin = new Vector2(0.5f, 0f);
            img.rectTransform.anchorMax = new Vector2(0.5f, 0f);
            img.rectTransform.pivot = new Vector2(0.5f, 0f);
            img.color = normalColor;

            _bars[i] = img;
            _curHeights[i] = minHeight;
            SetBarHeight(img, minHeight);
        }
    }

    void Update()
    {
        if (!source || _bars == null || _bars.Length == 0) return;

        if (fitMaxHeightToParent && barsParent)
            maxHeight = Mathf.Max(8f, barsParent.rect.height);

        // 1) Spectrum
        source.GetSpectrumData(_spectrum, 0, fftWindow);

        // 2) Map spectrum -> bars
        for (int i = 0; i < barCount; i++)
        {
            float value = SampleBand(i, barCount, _spectrum, useLogFrequencyScale);
            float targetHeight = Mathf.Clamp(minHeight + value * gain * (maxHeight - minHeight), minHeight, maxHeight);
            _curHeights[i] = Mathf.Lerp(targetHeight, _curHeights[i], smooth);
        }

        // 3) Perfect edge
        bool nowPerfect = IsPerfectNow();
        if (nowPerfect && !_wasPerfect)
        {
            _perfectPulse = true;
            _perfectHoldTimer = Mathf.Max(_perfectHoldTimer, perfectHoldSec);
        }
        _wasPerfect = nowPerfect;

        // 4) One-shot pulse
        if (_perfectPulse)
        {
            ApplyPerfectPulse();
            _perfectPulse = false;
        }

        // 5) Hold color cue (ChangeColor mode)
        if (_perfectHoldTimer > 0f)
        {
            _perfectHoldTimer -= Time.unscaledDeltaTime;
            if (perfectCue == PerfectCueMode.ChangeColor)
                for (int i = 0; i < _bars.Length; i++) _bars[i].color = perfectColor;
        }
        else
        {
            if (perfectCue == PerfectCueMode.ChangeColor)
                for (int i = 0; i < _bars.Length; i++) _bars[i].color = normalColor;
        }

        // 6) Apply heights
        for (int i = 0; i < _bars.Length; i++)
            SetBarHeight(_bars[i], _curHeights[i]);
    }

    void ApplyPerfectPulse()
    {
        switch (perfectCue)
        {
            case PerfectCueMode.AllBars:
                for (int i = 0; i < _bars.Length; i++)
                    _curHeights[i] = Mathf.Min(_curHeights[i] * Mathf.Max(1f, perfectMultiplier), maxHeight);
                break;

            case PerfectCueMode.EveryNthBar:
                int step = Mathf.Max(1, nth);
                for (int i = 0; i < _bars.Length; i++)
                    if (i % step == 0)
                        _curHeights[i] = Mathf.Min(_curHeights[i] * Mathf.Max(1f, perfectMultiplier), maxHeight);
                break;

            case PerfectCueMode.ChangeColor:
                // только цвет — высоту не изменяем
                break;
        }
    }

    static float SampleBand(int i, int totalBars, float[] spectrum, bool logScale)
    {
        int n = spectrum.Length;
        int from, to;

        if (logScale)
        {
            float f0 = Mathf.Pow((float)i / totalBars, 2f);
            float f1 = Mathf.Pow((float)(i + 1) / totalBars, 2f);
            from = Mathf.Clamp(Mathf.FloorToInt(f0 * n), 0, n - 1);
            to = Mathf.Clamp(Mathf.FloorToInt(f1 * n), from + 1, n);
        }
        else
        {
            from = Mathf.FloorToInt((i / (float)totalBars) * n);
            to = Mathf.FloorToInt(((i + 1f) / (float)totalBars) * n);
            to = Mathf.Max(to, from + 1);
        }

        float sum = 0f;
        for (int k = from; k < to; k++) sum += spectrum[k];
        float avg = sum / (to - from);
        return Mathf.Sqrt(avg);
    }

    void SetBarHeight(Image img, float h)
    {
        var rt = img.rectTransform;
        var size = rt.sizeDelta;
        size.y = h;
        rt.sizeDelta = size;
    }

    bool IsPerfectNow()
    {
        if (!conductor || !map || map.subBeatsSec == null || map.subBeatsSec.Length == 0)
            return false;

        float t = conductor.SongTime;
        int idx = map.NearestSubbeatIndex(t);
        if (idx < 0) return false;

        float secPerBeat = map.SecPerBeat;
        float dtSec = Mathf.Abs(map.subBeatsSec[idx] - t);
        float dtBeats = dtSec / secPerBeat;

        return dtBeats <= map.perfectWindowBeats;
    }

    void OnRectTransformDimensionsChange()
    {
        if (fitMaxHeightToParent && barsParent)
            maxHeight = Mathf.Max(8f, barsParent.rect.height);
    }
}
