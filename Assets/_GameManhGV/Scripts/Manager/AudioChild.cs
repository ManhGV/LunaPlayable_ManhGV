using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioChild : GameUnit
{
    [SerializeField] AudioSource _audioSource;
    AudioClip _audioClip;

    public void OnInit(AudioClip _audioClip, float _volume)
    {
        gameObject.name = "AudioChild: " + _audioClip.name;
        _audioSource.clip = _audioClip;
        _audioSource.volume = _volume;
        _audioSource.Play();
        Invoke(nameof(OnDespawn),_audioClip.length);
    }

    public void OnDespawn()
    {
        SimplePool.Despawn(this);
    }
}
