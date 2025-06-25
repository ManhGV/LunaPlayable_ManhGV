using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class OxygenTanks : MonoBehaviour, ITakeDamage
{
    [SerializeField] public int maxHealth = 100;
    
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _audioClip;
    
    [SerializeField] private GameObject _explosionGameObject;
    [SerializeField] private ParticleSystem _vfxExplosion;
    [SerializeField] private int _dame;
    
    [Header("Gizmod Explosion")]
    [SerializeField] private Transform _centerExplosion;
    [SerializeField] private float _radiusExplosion;
    [SerializeField] private LayerMask _layerHit;
    private Vector3 _direction;
    private float _lifeTimer = 5f;
    private bool CanExplosion = true;
    
    [FormerlySerializedAs("_meshRendererGas")]
    [Header("DisableIfExplosion")] 
    [SerializeField] private GameObject _body;
    [SerializeField] private CapsuleCollider _capsuleColliderThis;
    
    public void CheckHitExplosion()
    {
        Collider[] cols = Physics.OverlapSphere(_centerExplosion.position, _radiusExplosion, _layerHit);
        
        if (cols.Length <= 0)
            return;
            
        List<Transform> lstRoot = new List<Transform> ();
        foreach (Collider col in cols)
            if (!lstRoot.Contains(col.gameObject.transform.root))
                lstRoot.Add(col.gameObject.transform.root);
        
        foreach(var elem in lstRoot)
        {
            ZombieBase zombieBase = elem.gameObject.GetComponentInParent<ZombieBase>();
            
            if(zombieBase != null)
            {
                if(zombieBase is BotNetwork botnet)
                    botnet._posDamageGas = transform.position;
                
                var damageInfo = new DamageInfo()
                {
                    damageType = DamageType.Gas,
                    damage = _dame,
                    name = elem.gameObject.name,
                };
                zombieBase.TakeDamage(damageInfo);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!_centerExplosion) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_centerExplosion.position, _radiusExplosion);
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        maxHealth -= damageInfo.damage;
        if (maxHealth <= 0)
        {
            if(!CanExplosion)
                return;

            if (_explosionGameObject != null)
                _explosionGameObject.SetActive(true);
            CanExplosion = false;
            _body.SetActive(false);
            _capsuleColliderThis.enabled = false;
            _audioSource.PlayOneShot(_audioClip);
            _vfxExplosion.Play();
            CheckHitExplosion();
            Destroy(gameObject, _lifeTimer);
        }
    }

    public Transform GetTransformThis() => transform;
    public Transform GetTransformCenter() => _centerExplosion;
}
