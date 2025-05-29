using System;
using UnityEngine;

[Serializable]
public class Effect : GameUnit
{
    private float _timer;

    [SerializeField] private double _lifeTime;

    [SerializeField] public ParticleSystem[] particles;
    private Transform _parent;
    private Vector3 _localPosition;

    private bool haveParent = false;
    
    public void Init()
    {
        _timer = 0;
        foreach (var item in particles)
        {
            item.Play();
        }
        Update();
    }

    public void OnPushToPool()
    {
    }

    private void Update()
    {
        if (_timer > _lifeTime)
            OnDespawn();
        else
        {
            _timer += Time.deltaTime;
            if (haveParent)
            {
                TF.position = _parent.TransformPoint(_localPosition);   
            }
        }
    }

    public void OnDespawn()
    {
        SimplePool.Despawn(this);   
    }
}