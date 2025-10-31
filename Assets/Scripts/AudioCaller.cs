using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioCaller : MonoBehaviour
{
    public void PlaySFX(string soundName)
    {
        AudioManager.instance?.PlaySFX(soundName);
    }
    public void PlayMusic(string musicName)
    {
        AudioManager.instance?.PlayMusic(musicName);
    }
}
