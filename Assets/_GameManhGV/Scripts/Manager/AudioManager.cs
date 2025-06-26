using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] GameObject parentAudioChild;
    
    public void PlaySound(AudioClip _audioClip,float _volume)
    {
        if(GameManager.Instance.endGame)
            return;
        AudioChild _audioChild = SimplePool.Spawn<AudioChild>(GameConstants.PoolType.AudioChild,Vector3.zero,Quaternion.identity);
        _audioChild.OnInit(_audioClip, _volume);
    }
    
    public void StopAllAudio()
    {
        parentAudioChild.SetActive(false);
    }
}