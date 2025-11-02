using UnityEngine;

public class RhythmHooks : MonoBehaviour
{
    [Header("Refs")] public Weapon2D weapon;
    public ChaosMeter chaos;

    public MonoBehaviour rhythmJudgeComponent;

    private System.Action _onHit;
    private System.Action _onMiss;

    void OnEnable() { TrySubscribe(true); }
    void OnDisable() { TrySubscribe(false); }

    void TrySubscribe(bool subscribe)
    {
        if (rhythmJudgeComponent == null) return;
        var t = rhythmJudgeComponent.GetType();
        var evtHit = t.GetEvent("OnHit");
        var evtMiss = t.GetEvent("OnMiss");
        if (evtHit == null || evtMiss == null) return;

        _onHit = HandleHit;
        _onMiss = HandleMiss;

        if (subscribe) { evtHit.AddEventHandler(rhythmJudgeComponent, _onHit); evtMiss.AddEventHandler(rhythmJudgeComponent, _onMiss);} else { evtHit.RemoveEventHandler(rhythmJudgeComponent, _onHit); evtMiss.RemoveEventHandler(rhythmJudgeComponent, _onMiss);}
    }

    void HandleHit() { if (weapon) weapon.ShootTowardsMouse(); }
    void HandleMiss() { if (chaos) chaos.OnMiss(); }
}