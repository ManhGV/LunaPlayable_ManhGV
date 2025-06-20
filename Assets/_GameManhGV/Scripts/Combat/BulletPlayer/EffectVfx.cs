using System;
using UnityEngine;

[Serializable]
public class EffectVfx : EffectBase
{
    [SerializeField] private ParticleSystem[] particles;
    [SerializeField] private float _lifeTime;
    private float _timer;
 
    [Header("Size")]
    [SerializeField] float _maxSize = 1f;
    
    public override void OnInit()
    {
        base.OnInit();
        _timer = 0;
        foreach (var item in particles)
        {
            if(!item.gameObject.activeSelf)
                item.gameObject.SetActive(true);    
            item.Play();
        }
                
        Update();
    }
    
    public void OnInit(int _percentSize)//cho quả cầu có thể chỉnh to nhỏ theo phần trăm
    {
        base.OnInit();
        TF.localScale =Vector3.one * (_maxSize * _percentSize/100f);
        
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
}