using UnityEngine;
using UnityEngine.Events;

public enum HitQuality { Miss, Good, Perfect }

public class RhythmJudge : MonoBehaviour
{
    public BeatConductor conductor;
    public BeatMapData map;

    [Header("Events")]
    public UnityEvent OnGoodHit;
    public UnityEvent OnPerfectHit;
    public UnityEvent OnMiss;

    void Start()
    {
        if (!map && conductor) map = conductor.map;
    }

    public HitQuality JudgeNow()
    {
        if (conductor == null || map == null || map.subBeatsSec == null || map.subBeatsSec.Length == 0)
            return HitQuality.Miss;

        float t = conductor.SongTime;
        int idx = map.NearestSubbeatIndex(t);
        if (idx < 0) return HitQuality.Miss;

        float dtSec = Mathf.Abs(map.subBeatsSec[idx] - t);
        float secPerBeat = map.SecPerBeat;

        float dtBeats = dtSec / secPerBeat;
        if (dtBeats <= map.perfectWindowBeats) { OnPerfectHit?.Invoke(); return HitQuality.Perfect; }
        if (dtBeats <= map.goodWindowBeats) { OnGoodHit?.Invoke(); return HitQuality.Good; }
        OnMiss?.Invoke(); return HitQuality.Miss;
    }
}
