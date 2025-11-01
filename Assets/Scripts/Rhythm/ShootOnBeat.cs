using UnityEngine;
// новый инпут:
using UnityEngine.InputSystem;

public class ShootOnBeat : MonoBehaviour
{
    public RhythmJudge judge;

    void Update()
    {
        bool attackPressed =
            (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) ||
            (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame);

        if (!attackPressed) return;

        var res = judge.JudgeNow();
        if (res == HitQuality.Perfect || res == HitQuality.Good)
        {
            // TODO: ваша атака/пуля/удар
            Debug.Log($"HIT: {res}");
        }
        else
        {
            Debug.Log("MISS");
        }
    }
}
