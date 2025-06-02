using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionPool : GameUnit
{
    [SerializeField] private ParticleSystem vfxExplosion;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _audioExplosion;
    public void OnInit()
    {
        vfxExplosion.Play();
        _audioSource.PlayOneShot(_audioExplosion);
    }

    public void OnDespawn(float _delay) => StartCoroutine(IEDelayDespawn(_delay));
    
    public void OnDespawn()
    {
        vfxExplosion.Stop();
        SimplePool.Despawn(this);
    }
    
    IEnumerator IEDelayDespawn(float delay)
    {
        yield return new WaitForSeconds(delay);
        vfxExplosion.Stop();
        SimplePool.Despawn(this);
    }
}