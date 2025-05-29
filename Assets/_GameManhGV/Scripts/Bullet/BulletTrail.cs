//using UnityEditor.Presets;
using UnityEngine;

public class BulletTrail : GameUnit
{
    [SerializeField] protected Transform _trail;
    [SerializeField] protected MeshRenderer[] meshRenderers;
    [SerializeField] protected Material[] materials;
    [SerializeField] protected Vector3 _trailStartScale;
    [SerializeField] protected Vector3 _trailMaxScale;
    [SerializeField] protected float _trailLengthAtMaxScale;
    public float Speed;
    protected Vector3 _direction;
    protected float _traveledDistance;
    protected float _maxDistance;
    public float LifeTime = 2f;
    protected float _lifeTimer;

    public Vector3 posE;

    private void Awake()
    {
        posE = transform.forward * 1;
    }
    
    public void Init(Vector3 direction, Vector3 posE)
    {
        this.posE = posE;
        _lifeTimer = 0;
        if (_trail) 
            _trail.localScale = _trailStartScale;
        
        gameObject.SetActive(true);
        _direction = direction;
        TF.rotation = Quaternion.LookRotation(_direction);
        _maxDistance = Speed * LifeTime;
        _traveledDistance = 0;
        Update();
    }
    public void Init(Vector3 direction)
    {
        _lifeTimer = 0;
        if (_trail) 
            _trail.localScale = _trailStartScale;
        
        gameObject.SetActive(true);
        
        _direction = direction;
        TF.rotation = Quaternion.LookRotation(_direction);
        _maxDistance = Speed * LifeTime;
        _traveledDistance = 0;
        Update();
    }

    // Update is called once per frame
    protected void Update()
    {
        if(transform.position.z > posE.z)
            OnDespawn();
        
        if (_traveledDistance >= _maxDistance || _lifeTimer > LifeTime)
            OnDespawn();
        else
        {
            _lifeTimer += Time.deltaTime;
            var movement = Speed * Time.deltaTime;
            TF.position += _direction * movement;
            _traveledDistance += movement;
            if (_trail)
                _trail.localScale = Vector3.Lerp(_trailStartScale, _trailMaxScale,
                    _traveledDistance / _trailLengthAtMaxScale);
        }
    }
    
    public void OnDespawn()
    {
        _trail.localScale = _trailStartScale;
        Speed = 300;
        
        SimplePool.Despawn(this);
    }
}