using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip ButtonBasicClip => _buttonBasicClip;
    [SerializeField] private AudioClip _buttonBasicClip;


    public enum SoundType { BGM, SFX }
    private GameObject _basicGameObject;
    private Dictionary<SoundType, Sound> _sounds = new();
    private AudioSource _currentBGM;

    private const string SourceName = "Source";
    private const string RootName = "Sound";
    private const string BGMName = "BGM";
    private const string SFXName = "SFX";

    public void Initialize()
    {
        var root = CreateNewGameObject(RootName, transform).transform;

        _basicGameObject = new GameObject(SourceName);
        _basicGameObject.transform.SetParent(root);
        var source = _basicGameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        
        var bgm = CreateNewGameObject(BGMName, root).transform;
        var sfx = CreateNewGameObject(SFXName, root).transform;

        _sounds.Add(SoundType.BGM, new Sound
        {
            Root = bgm,
            volume = 1,
            Pool = new GameObjectPool<AudioSource>(_basicGameObject, bgm)
        });

        _sounds.Add(SoundType.SFX, new Sound
        {
            Root = sfx,
            volume = 1,
            Pool = new GameObjectPool<AudioSource>(_basicGameObject, sfx)
        });
    }

    private GameObject CreateNewGameObject(string name, Transform parent)
    {
        var transform = new GameObject(name).transform;
        transform.SetParent(parent);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        return transform.gameObject;
    }

    public void PlaySFX(AudioClip clip)
    {
        var source = _sounds[SoundType.SFX].Pool.Get();
        source.clip = clip;
        source.loop = false;
        source.Play();
        StartCoroutine(ReturnRoutine(SoundType.SFX, source));
    }

    public void PlayBGM(AudioClip clip)
    {
        StopBGM();
        _currentBGM = _sounds[SoundType.BGM].Pool.Get();
        _currentBGM.clip = clip;
        _currentBGM.loop = true;
        _currentBGM.Play();
    }

    public void StopBGM()
    {
        if (_currentBGM != null)
        {
            _currentBGM.Stop();
            _sounds[SoundType.BGM].Pool.Release(_currentBGM);
        }
    }

    public void PlayBaiscButton()
    {
        PlaySFX(_buttonBasicClip);
    }

    private IEnumerator ReturnRoutine(SoundType type, AudioSource source)
    {
        yield return new WaitForSecondsRealtime(source.clip.length);
        source.clip = null;
        _sounds[type].Pool.Release(source);
    }

    public void SetVolume(SoundType type, float volume)
    {
        _sounds[type].SetVolumn(volume);
    }
}
