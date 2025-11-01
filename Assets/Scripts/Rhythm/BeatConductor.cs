using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BeatConductor : MonoBehaviour
{
    public BeatMapData map;
    public float userLatencyMs = 0f; // настройка игрока (если нужно)
    public double scheduleLeadSec = 0.1; // зазор перед стартом

    private AudioSource _src;
    private double _dspStart = -1;

    public float SongTime
    {
        get
        {
            if (_dspStart < 0) return 0f;
            return (float)(AudioSettings.dspTime - _dspStart) + userLatencyMs / 1000f;
        }
    }

    void Awake()
    {
        _src = GetComponent<AudioSource>();
        _src.playOnAwake = false;
    }

    public void Play(BeatMapData newMap = null)
    {
        if (newMap != null) map = newMap;
        if (map == null || map.clip == null) { Debug.LogError("BeatConductor: map/clip missing"); return; }

        _src.Stop();
        _src.clip = map.clip;

        var when = AudioSettings.dspTime + scheduleLeadSec;
        _src.PlayScheduled(when);
        _dspStart = when;
    }

    public void Stop()
    {
        _src.Stop();
        _dspStart = -1;
    }
}
