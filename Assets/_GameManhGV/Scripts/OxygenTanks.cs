using System.Collections.Generic;
using UnityEngine;

public class OxygenTanks : MonoBehaviour
{
    [SerializeField] public int countExplosion = 4;
    
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _audioClip;
    [SerializeField] private ParticleSystem _vfxExplosion;
    [SerializeField] private int _dame;
    
    [Header("Gizmod Explosion")]
    [SerializeField] private Transform _centerExplosion;
    [SerializeField] private float _radiusExplosion;
    [SerializeField] private LayerMask _layerHit;
    private Vector3 _direction;
    private float _lifeTimer = 5f;
    private bool CanExplosion = true;

    [Header("DisableIfExplosion")] 
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private CapsuleCollider _capsuleCollider;
    
    public void Explosion()
    {
        if (countExplosion > 0)
        {
            countExplosion--;
            return;
        }
        
        if(!CanExplosion)
            return;
        
        CanExplosion = false;
        _meshRenderer.enabled = false;
        _capsuleCollider.enabled = false;
        _audioSource.PlayOneShot(_audioClip);
        _vfxExplosion.Play();
        CheckHitExplosion();
        Destroy(gameObject, _lifeTimer);
    }
    
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
}
