using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ProjectileExplosionBase : GameUnit
{
    [SerializeField] protected AudioSource _audioSource; //âm thanh move
    [SerializeField] protected ParticleSystem _moveEffect;
    [SerializeField] protected GameConstants.PoolType _poolTypeExplosion; // Loại Pool của Projectile
    [SerializeField] protected int _damage;
    [SerializeField] protected float _lifeTime;
    [SerializeField] protected float _speed;
    
    [Header("Explosion")]
    [SerializeField] protected float _maxRadiusCollider;
    [SerializeField] protected float _maxRadiusExplosion;
    protected float _radiusCollider;
    protected float _radiusExplosion;
    [SerializeField] protected LayerMask _layerHit;
    private Vector3 _direction;
    private float _lifeTimer;
    private bool _isHitCollider;
    private int _percentSize;
    public virtual void OnInit(Vector3 _direction, int _percentSize)
    {
        _radiusCollider = _maxRadiusCollider*(_percentSize / 100f);
        _radiusExplosion = _maxRadiusExplosion*(_percentSize / 100f);
        this._percentSize = _percentSize;
        this._direction = _direction;
        transform.LookAt(_direction);
        _isHitCollider = false;
        _lifeTimer = 0;
        transform.rotation = Quaternion.Euler(Vector3.zero);
    }
    
    public virtual void OnInit(Vector3 _PointEnd)
    {
        _direction = _PointEnd;
        _direction = (_direction - transform.position).normalized;
        transform.LookAt(_direction);
        _isHitCollider = false;
        _lifeTimer = 0;
        transform.rotation = Quaternion.Euler(Vector3.zero);
    }

    protected virtual void OnEnable()
    {
        _audioSource?.Play();
        _moveEffect?.Play();
    }
    protected virtual void OnDisable()
    {
        _audioSource?.Stop();
        _moveEffect?.Stop();
    }

    protected virtual void Update()
    {
        _lifeTimer += Time.deltaTime;
        if(_lifeTimer > _lifeTime)
        {
            Explosion();
        }
        else
        {
            if(!_isHitCollider)
            {
                NormalFire();
                CheckHitCollider();
            }
        }

    }
    public virtual void NormalFire()
    {
        transform.position += _direction * _speed * Time.deltaTime;
    }
    
    public virtual void Explosion()
    {
        OnDespawn();
        
        //RocketController.Instance.PlayAudioExplosion();
        EffectVfx explosionPool = SimplePool.Spawn<EffectVfx>(_poolTypeExplosion, TF.position, Quaternion.Euler(-90,0,0));
        explosionPool.OnInit(_percentSize);
        //RocketController.Instance.SnakeCameraRocket();
    }
    
    public virtual void CheckHitCollider()
    {
        Collider[] cols = Physics.OverlapSphere(TF.position, _radiusCollider, _layerHit);
        if(cols.Length != 0 )
        {
            _isHitCollider = true;
            Explosion();
            CheckHitExplosion();
        }
    }
    public virtual void CheckHitExplosion()
    {
        Collider[] cols = Physics.OverlapSphere(TF.position, _radiusExplosion, _layerHit);
        List<Transform> lstRoot = new List<Transform> ();
        foreach (Collider col in cols)
        {
            if (!lstRoot.Contains(col.gameObject.transform.root))
            {
                lstRoot.Add(col.gameObject.transform.root);
            }
        }
        foreach(var elem in lstRoot)
        {
            // damageType = elem.CompareTag("WeakPoint") ? DamageType.Weekness : DamageType.Normal;
            ITakeDamage iTakeDamage = elem.gameObject.GetComponentInParent<ITakeDamage>();
            if (iTakeDamage != null)
            {
                var damageInfo = new DamageInfo()
                {
                    damageType = DamageType.Gas,
                    damage = _damage,
                    name = elem.gameObject.name,
                };
                iTakeDamage.TakeDamage(damageInfo);
            }
        }
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(TF.position, _radiusExplosion);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(TF.position, _radiusCollider);
    }

    public virtual void OnDespawn()
    {
        SimplePool.Despawn(this);
    }
}