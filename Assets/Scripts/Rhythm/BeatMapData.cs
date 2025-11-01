using UnityEngine;

[CreateAssetMenu(menuName = "Rhythm/Beat Map Data", fileName = "BeatMapData")]
public class BeatMapData : ScriptableObject
{
    public AudioClip clip;

    [Header("From JSON")]
    public float bpm = 120f;
    public float offsetSecApplied = 0f; // смещение, которое уже применено к таймкодам
    public float[] beatsSec;
    public float[] subBeatsSec;

    [Header("Judging windows (in beats)")]
    public float perfectWindowBeats = 0.06f;
    public float goodWindowBeats = 0.12f;

    public float SecPerBeat => 60f / Mathf.Max(1e-3f, bpm);

    /// Возвращает индекс ближайшего саббита к указанному времени песни (сек)
    public int NearestSubbeatIndex(float songTime)
    {
        if (subBeatsSec == null || subBeatsSec.Length == 0) return -1;
        int i = System.Array.BinarySearch(subBeatsSec, songTime);
        if (i >= 0) return i;
        i = ~i; // нижняя вставка
        if (i <= 0) return 0;
        if (i >= subBeatsSec.Length) return subBeatsSec.Length - 1;
        return (songTime - subBeatsSec[i - 1]) <= (subBeatsSec[i] - songTime) ? i - 1 : i;
    }
}
