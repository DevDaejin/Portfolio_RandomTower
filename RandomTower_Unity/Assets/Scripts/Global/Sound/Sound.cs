using System.Collections.Generic;
using UnityEngine;

public class Sound
{
    public Transform Root;
    public float volume;
    public GameObjectPool<AudioSource> Pool;

    public void SetVolumn(float volume)
    {
        this.volume = volume;
        var audios = Root.GetComponentsInChildren<AudioSource>(true);

        foreach(var audio in audios)
        {
            audio.volume = this.volume;
        }
    }
}
