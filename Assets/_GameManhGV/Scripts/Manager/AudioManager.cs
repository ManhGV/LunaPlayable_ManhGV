using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    public void PlaySound(AudioClip _audioClip,float _volume)
    {
        if(GameManager.Instance.GetGameState() == GameConstants.GameState.EndGame)
            return;
        AudioChild _audioChild = SimplePool.Spawn<AudioChild>(GameConstants.PoolType.AudioChild,Vector3.zero,Quaternion.identity);
        _audioChild.OnInit(_audioClip, _volume);
    }
}