using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectBase : GameUnit
{
#if UNITY_EDITOR
    [SerializeField] private GameConstants.EffectType effectType;
    private void OnValidate()
    {
        PoolType = (GameConstants.PoolType)effectType;
    }
#endif
    [Header("Audio")] [SerializeField] private bool playEqualManager = false;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _audioClip;
    public virtual void OnInit()
    {
        if (playEqualManager)
        {
            AudioManager.Instance.PlaySound(_audioClip,.5f);
        }
        else
        {
            if (_audioSource != null)
                _audioSource.PlayOneShot(_audioClip);
        }
        // Override this method in derived classes to implement specific initialization logic
    }
    
    public virtual void OnDespawn()
    {
        if (_audioSource != null)
            _audioSource.Stop();
        SimplePool.Despawn(this);
    }
}