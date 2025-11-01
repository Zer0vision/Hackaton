using UnityEngine;
using UnityEngine.UI;

public class ChaosMeter : MonoBehaviour
{
    [Range(0f, 1f)] public float Value = 0f;
    [Header("Tuning")] public float missIncrement = 0.12f;
    public float decayPerSecond = 0.03f;

    [Header("UI")] public Image barFill;

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
        barFill.color = Color.Lerp(Color.green, Color.red, Value);
    }
}