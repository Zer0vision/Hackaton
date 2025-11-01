#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

public class BeatMapImporterWindow : EditorWindow
{
    [Serializable]
    private class WindowsJSON { public float perfect_beats; public float good_beats; }

    [Serializable]
    private class BeatmapJSON
    {
        public string source;
        public int sr;
        public float duration_sec;
        public float bpm;
        public float offset_sec_applied;
        public float[] beats_sec;
        public int subdiv;
        public float[] subbeats_sec;
        public float[] onsets_sec;
        public WindowsJSON windows;
    }

    private TextAsset jsonFile;
    private AudioClip audioClip;
    private string assetName = "SongData";

    [MenuItem("Tools/Rhythm/Import BeatMap JSON")]
    private static void Open() => GetWindow<BeatMapImporterWindow>("BeatMap Importer");

    private void OnGUI()
    {
        EditorGUILayout.LabelField("BeatMap JSON → ScriptableObject", EditorStyles.boldLabel);
        jsonFile = (TextAsset)EditorGUILayout.ObjectField("JSON File", jsonFile, typeof(TextAsset), false);
        audioClip = (AudioClip)EditorGUILayout.ObjectField("Audio Clip", audioClip, typeof(AudioClip), false);
        assetName = EditorGUILayout.TextField("Asset Name", assetName);

        GUI.enabled = jsonFile != null && audioClip != null && !string.IsNullOrEmpty(assetName);
        if (GUILayout.Button("Import"))
        {
            try
            {
                var data = JsonUtility.FromJson<BeatmapJSON>(jsonFile.text);
                var asset = ScriptableObject.CreateInstance<BeatMapData>();
                asset.clip = audioClip;
                asset.bpm = data.bpm;
                asset.offsetSecApplied = data.offset_sec_applied;
                asset.beatsSec = data.beats_sec;
                asset.subBeatsSec = data.subbeats_sec;
                if (data.windows != null)
                {
                    asset.perfectWindowBeats = data.windows.perfect_beats;
                    asset.goodWindowBeats = data.windows.good_beats;
                }

                var path = $"Assets/{assetName}.asset";
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();
                EditorUtility.DisplayDialog("BeatMap Import", $"Created: {path}", "OK");
                Selection.activeObject = asset;
            }
            catch (Exception e)
            {
                Debug.LogError($"BeatMap import failed: {e}");
                EditorUtility.DisplayDialog("Error", e.Message, "OK");
            }
        }
        GUI.enabled = true;
    }
}
#endif
