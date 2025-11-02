using UnityEngine;
using UnityEngine.UI;

public class ChaosMeter : MonoBehaviour
{
    [Range(0f, 1f)] public float Value = 0f;

    [Header("Tuning")] public float missIncrement = 0.12f;
    public float decayPerSecond = 0.03f;

    [Header("Colors")]
    public Color lowColor = new Color(0.2f, 1f, 0.2f);   // зелёный
    public Color redColor = new Color(1f, 0.2f, 0.2f);   // красный
    public Color graphiteColor = new Color32(43, 43, 46, 255); // графитовый (не чистый чёрный)
    [Range(0f, 1f)] public float graphiteStart = 0.75f;  // после этого начинаем переход от красного к графиту

    [Header("UI")] public Image barFill;

    void Awake()
    {
        // Гарантируем вертикальное заполнение снизу вверх
        if (barFill) {
            barFill.type = Image.Type.Filled;
            barFill.fillMethod = Image.FillMethod.Vertical;
            barFill.fillOrigin = (int)Image.OriginVertical.Bottom;
            ApplyUI();
        }
    }

    void Update()
    {
        if (Value > 0f)
        {
            Value = Mathf.Max(0f, Value - decayPerSecond * Time.deltaTime);
            ApplyUI();
        }
    }

    public void OnMiss() => AddChaos(missIncrement);

    public void AddChaos(float amount)
    {
        Value = Mathf.Clamp01(Value + amount);
        ApplyUI();
    }

    public void ResetChaos()
    {
        Value = 0f;
        ApplyUI();
    }

    void ApplyUI()
    {
        if (!barFill) return;
        barFill.fillAmount = Value;
        barFill.color = EvaluateColor(Value);
    }

    Color EvaluateColor(float t)
    {
        // 0 .. graphiteStart  — плавно зелёный -> красный
        // graphiteStart .. 1 — плавно красный -> графитовый
        if (t <= graphiteStart)
        {
            float u = Mathf.InverseLerp(0f, graphiteStart, t);
            return Color.Lerp(lowColor, redColor, u);
        }
        else
        {
            float u = Mathf.InverseLerp(graphiteStart, 1f, t);
            return Color.Lerp(redColor, graphiteColor, u);
        }
    }
}
