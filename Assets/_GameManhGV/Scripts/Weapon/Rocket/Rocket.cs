using System.Collections.Generic;
using UnityEngine;
public class Rocket : GameUnit
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private ParticleSystem _fireEffect;
    [SerializeField] private int _dame;
    [SerializeField] private float _lifeTime;
    [SerializeField] private float _speed;
    
    [Header("Explosion")]
    [SerializeField] private float _radiusColli;
    [SerializeField] private float _radiusExplosion;
    [SerializeField] private LayerMask _layerHit;
    private Vector3 _direction;
    private float _lifeTimer;
    private bool _isHitCollider;
    
    public void OnInit(Vector3 direction)
    {
        _direction = direction;
        _direction = (_direction - transform.position).normalized;
        transform.LookAt(_direction);
        _isHitCollider = false;
        _lifeTimer = 0;
        transform.rotation = Quaternion.Euler(Vector3.zero);
    }

    private void OnEnable()
    {
        _audioSource?.Play();
        _fireEffect?.Play();
    }
    private void OnDisable()
    {
        _audioSource?.Stop();
        _fireEffect?.Stop();
    }

    void Update()
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
    public void NormalFire()
    {
        transform.position += _direction * _speed * Time.deltaTime;
    }
    
    public void Explosion()
    {
        OnDespawn();
        
        RocketController.Instance.PlayAudioExplosion();
        ExplosionPool explosionPool = SimplePool.Spawn<ExplosionPool>(GameConstants.PoolType.vfx_ExplosionRocket, TF.position, Quaternion.identity);
        explosionPool.OnInit();
        explosionPool.OnDespawn(3f);   
        RocketController.Instance.SnakeCameraRocket();
    }
    
    public void CheckHitCollider()
    {
        Collider[] cols = Physics.OverlapSphere(TF.position, _radiusColli, _layerHit);
        if(cols.Length != 0 )
        {
            _isHitCollider = true;
            Explosion();
            CheckHitExplosion();
        }
    }
    public void CheckHitExplosion()
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
            var takeDamageController = elem.gameObject.GetComponentInParent<ITakeDamage>();
            if(takeDamageController != null)
            {
                // damageType = elem.CompareTag("WeakPoint") ? DamageType.Weekness : DamageType.Normal;
                BotNetwork botnet = elem.gameObject.GetComponentInParent<BotNetwork>();
                if (botnet != null)
                {
                    //botnet.posExplosion = transform.position;
                }
                
                var damageInfo = new DamageInfo()
                {
                    damageType = DamageType.Gas,
                    damage = _dame,
                    name = elem.gameObject.name,
                };
                takeDamageController.TakeDamage(damageInfo);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(TF.position, _radiusExplosion);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(TF.position, _radiusColli);
    }

    public void OnDespawn()
    {
        SimplePool.Despawn(this);
    }
}
