using System.Collections.Generic;
using UnityEngine;
public class ProjectileExplosion : GameUnit
{
    [SerializeField] protected AudioSource _audioSource;
    [SerializeField] protected ParticleSystem _fireEffect;
    [SerializeField] protected int _dame;
    [SerializeField] protected float _lifeTime;
    [SerializeField] protected float _speed;
    
    [Header("Explosion")]
    [SerializeField] protected float _radiusColli;
    [SerializeField] protected float _radiusExplosion;
    [SerializeField] protected LayerMask _layerHit;
    protected Vector3 _direction;
    protected float _lifeTimer;
    protected bool _isHitCollider;
    
    public virtual void OnInit(Vector3 direction)
    {
        _direction = (direction - TF.position).normalized;
        TF.LookAt(direction);
        _isHitCollider = false;
        _lifeTimer = 0;
    }

    protected virtual void OnEnable()
    {
        _audioSource?.Play();
        _fireEffect?.Play();
    }
    protected virtual void OnDisable()
    {
        _audioSource?.Stop();
        _fireEffect?.Stop();
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
        
        RocketController.Instance.PlayAudioExplosion();
        EffectVfx explosionPool = SimplePool.Spawn<EffectVfx>(GameConstants.PoolType.vfx_ExplosionRocket, TF.position, Quaternion.identity);
        explosionPool.OnInit();
        RocketController.Instance.SnakeCameraRocket();
    }
    
    public virtual void CheckHitCollider()
    {
        Collider[] cols = Physics.OverlapSphere(TF.position, _radiusColli, _layerHit);
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
                lstRoot.Add(col.gameObject.transform.root);
        }
        foreach(var elem in lstRoot)
        {
                // damageType = elem.CompareTag("WeakPoint") ? DamageType.Weekness : DamageType.Normal;
            ITakeDamage iTakeDamage = elem.gameObject.GetComponentInParent<ITakeDamage>();
            if(iTakeDamage == null)
                iTakeDamage = elem.gameObject.GetComponent<ITakeDamage>();
            
            if (iTakeDamage != null)
            {
                print(iTakeDamage.GetTransformThis().gameObject.name);
                var damageInfo = new DamageInfo()
                {
                    damageType = DamageType.Gas,
                    damage = _dame,
                    name = elem.gameObject.name,
                };
                iTakeDamage.TakeDamage(damageInfo);
                // if (iTakeDamage is BossZomSwatNetword boss)
                // {
                //     boss.ExplosinArrmor();
                // }
            }
        }
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(TF.position, _radiusExplosion);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(TF.position, _radiusColli);
    }

    public virtual void OnDespawn()
    {
        SimplePool.Despawn(this);
    }
}
