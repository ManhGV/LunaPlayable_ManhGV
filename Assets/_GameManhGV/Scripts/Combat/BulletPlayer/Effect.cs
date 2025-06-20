using System;
using UnityEngine;

[Serializable]
public class Effect : GameUnit
{
#if UNITY_EDITOR
    [SerializeField] private GameConstants.EffectType effectType;
    private void OnValidate()
    {
        PoolType = (GameConstants.PoolType)effectType;
    }
#endif
    
    [SerializeField] private double _lifeTime;
    private float _timer;

    [SerializeField] private ParticleSystem[] particles;
    
    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _audioClip;
 
    [Header("Size")]
    [SerializeField] float _maxSize = 1f;
    
    [Header("Sét đánh")]
    [SerializeField] LineRenderer _lineRenderer;
    [SerializeField] float _linedistance = 5f; // Khoảng cách giữa các điểm trên đường thẳng
    float xOffset = 0;
    private Transform posStart;
    private Transform posEnd;
    
    
    public void OnInit()
    {
        if (_audioSource != null)
            _audioSource.PlayOneShot(_audioClip);
        
        _timer = 0;
        foreach (var item in particles)
        {
            if(!item.gameObject.activeSelf)
                item.gameObject.SetActive(true);    
            item.Play();
        }
                
        Update();
    }

    public void OnInit(Transform posStart, Transform posEnd)
    {
        _timer = 0;
        this.posStart = posStart;
        this.posEnd = posEnd;
    }
    
    public void OnInit(int _percentSize)//cho quả cầu có thể chỉnh to nhỏ theo phần trăm
    {
        TF.localScale =Vector3.one * (_maxSize * _percentSize/100f);
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
            
        
        if(_lineRenderer== null) 
            return;
        if(Vector3.Distance(posStart.position, posEnd.position) > _linedistance)
            OnDespawn();
        
        _lineRenderer.SetPosition(0, posStart.position);
        _lineRenderer.SetPosition(1, posEnd.position);
        
        xOffset += Time.deltaTime*9;
        if (xOffset>=0.6) xOffset = 0;
        _lineRenderer.material.mainTextureOffset = new Vector2(xOffset, 0);
    }

    public void OnDespawn()
    {
        SimplePool.Despawn(this);
    }
}