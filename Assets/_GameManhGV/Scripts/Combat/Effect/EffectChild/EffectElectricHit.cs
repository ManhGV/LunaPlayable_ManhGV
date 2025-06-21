using System.Net.Http.Headers;
using UnityEngine;

public class EffectElectricHit : EffectBase
{
    [SerializeField] private ParticleSystem[] particles;
    [SerializeField] private float _lifeTime = 4f;
    private float _timer;
    bool _canCaculatorTimeDespawn = false;
    
    public void OnInit(float _scale, Transform _parent)
    {
        base.OnInit();
        _timer = _lifeTime;
        _canCaculatorTimeDespawn = false;
        TF.parent = _parent;
        TF.localPosition = Vector3.zero;
        TF.localRotation =Quaternion.Euler(Vector3.zero);
        foreach (var item in particles)
        {
            TF.localScale = Vector3.one * _scale;
            item.Play();
        }
        Update();
    }

    public void StartCaculatorTimeDespawn() => _canCaculatorTimeDespawn = true;
    
    private void Update()
    {
        if(!_canCaculatorTimeDespawn)
            return;
        
        _timer -= Time.deltaTime;
        if (_timer <= 0)
            OnDespawn();
    }

    public override void OnDespawn()
    {
        base.OnDespawn();
        TF.parent = null;
    }
}