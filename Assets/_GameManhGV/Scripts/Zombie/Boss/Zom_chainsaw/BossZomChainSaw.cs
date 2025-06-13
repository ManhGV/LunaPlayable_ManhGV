using static GameConstants;
using System.Collections.Generic;
using UnityEngine;

public class BossZomChainSaw : StateControllerBase<BossNetwork>
{
    [Header("Hit")] 
    [SerializeField]private int countBulletToHit;
    [SerializeField]private int countBullet;
    
    BossZomChainSaw_Idle _idleState;
    BossZomChainSaw_Attack _attackState;
    BossZomChainSaw_Move _moveState;
    BossZomChainSaw_Hit _hitState;
    BossZomChainSaw_Dead _deadState;
    
    [Header("Explosion")]
    public Transform _centerExplosion;
    public float _radiusExplosion;
    public LayerMask _layerHit;
    
    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _idleState = GetComponent<BossZomChainSaw_Idle>();
        _idleState.Initialize(ZomAllState.Idle);
        
        _moveState = GetComponent<BossZomChainSaw_Move>();
        _moveState.Initialize(ZomAllState.Move);

        _attackState = GetComponent<BossZomChainSaw_Attack>();
        _attackState.Initialize(ZomAllState.Attack);

        _hitState = GetComponent<BossZomChainSaw_Hit>();
        _hitState.Initialize(ZomAllState.Stun_1);
        
        _deadState = GetComponent<BossZomChainSaw_Dead>();
        _deadState.Initialize(ZomAllState.Dead);

        stateController.Add(ZomAllState.Idle, _idleState);
        stateController.Add(ZomAllState.Move, _moveState);
        stateController.Add(ZomAllState.Attack, _attackState);
        stateController.Add(ZomAllState.Stun_1, _hitState);
        stateController.Add(ZomAllState.Dead, _deadState);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _currentState = stateController[ZomAllState.Idle];
        _currentState.EnterState();
    }


    public bool CanHit()
    {
        if (countBullet >= countBulletToHit)
        {
            countBullet = 0;
            return true;
        }
        
        return false;
    }
    
    public void PlusBulletToHit()=>countBullet++;
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
//        Gizmos.DrawWireSphere(_centerExplosion.position, _radiusExplosion);
    }
    
    public void CheckHitColliderHitExplosion()
    {
        Collider[] cols = Physics.OverlapSphere(_centerExplosion.position, _radiusExplosion, _layerHit);
        if(cols.Length != 0 )
        {
            CheckHitExplosion();
        }
    }
    public void CheckHitExplosion()
    {
        Collider[] cols = Physics.OverlapSphere(_centerExplosion.position, _radiusExplosion, _layerHit);
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
            // BotNetwork botnet = elem.gameObject.GetComponentInParent<BotNetwork>();
            // if (botnet != null)
            // {
            //     botnet.posExplosion = transform.position;
            // }
            
            if(takeDamageController != null)
            {
                var damageInfo = new DamageInfo()
                {
                    damageType = DamageType.Gas,
                    damage = 10000,
                    name = elem.gameObject.name,
                };
                takeDamageController.TakeDamage(damageInfo);
            }
        }
    }
    
}