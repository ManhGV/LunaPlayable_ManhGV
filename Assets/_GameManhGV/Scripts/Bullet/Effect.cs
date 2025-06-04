using System;
using UnityEngine;

[Serializable]
public class Effect : GameUnit
{
    [SerializeField] private double _lifeTime;
    private float _timer;

    [SerializeField] public ParticleSystem[] particles;
    
    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _audioClip;
    
    public void OnInit()
    {
        if (_audioSource != null)
            _audioSource.PlayOneShot(_audioClip);
        
        _timer = 0;
        foreach (var item in particles)
            item.Play();
                
        Update();
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer > _lifeTime)
            OnDespawn();
    }

    public void OnDespawn()
    {
        SimplePool.Despawn(this);   
    }
}