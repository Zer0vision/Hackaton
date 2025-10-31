using UnityEngine;

public class AudioCaller : MonoBehaviour
{
    public void PlaySFX(string soundName)
    {
        if (AudioManager.TryGetInstance(out var manager))
        {
            manager.PlaySFX(soundName);
        }
    }

    public void PlayMusic(string musicName)
    {
        if (AudioManager.TryGetInstance(out var manager))
        {
            manager.PlayMusic(musicName);
        }
    }
}
