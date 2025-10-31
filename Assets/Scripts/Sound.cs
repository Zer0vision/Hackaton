using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class Sound
{
    [FormerlySerializedAs("name")]
    [SerializeField] private string id;

    [FormerlySerializedAs("clip")]
    [SerializeField] private AudioClip audioClip;

    public string Id => id;
    public AudioClip Clip => audioClip;
}
