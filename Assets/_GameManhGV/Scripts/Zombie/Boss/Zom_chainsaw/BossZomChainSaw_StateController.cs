using static GameConstants;
using System.Collections.Generic;
using UnityEngine;

public class BossZomChainSaw_StateController : StateControllerBase<BossZomChainSaw_NetWork>
{
    BossZomChainSaw_Idle _idleState;
    BossZomChainSaw_Attack _attackState;
    BossZomChainSaw_Move _moveState;
    BossZomChainSaw_Stun _stunState;
    BossZomChainSaw_Scream _screamState;
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
        _idleState.Initialize(ZomAllState.Idle, botNetworks,this);
        
        _moveState = GetComponent<BossZomChainSaw_Move>();
        _moveState.Initialize(ZomAllState.Move, botNetworks,this);
        
        _attackState = GetComponent<BossZomChainSaw_Attack>();
        _attackState.Initialize(ZomAllState.Attack, botNetworks,this);
        
        _stunState = GetComponent<BossZomChainSaw_Stun>();
        _stunState.Initialize(ZomAllState.Stun_1, botNetworks,this);
        
        _deadState = GetComponent<BossZomChainSaw_Dead>();
        _deadState.Initialize(ZomAllState.Dead, botNetworks,this);
        
        _screamState = GetComponent<BossZomChainSaw_Scream>();
        _screamState.Initialize(ZomAllState.Scream, botNetworks,this);
        
        stateController.Add(ZomAllState.Idle, _idleState);
        stateController.Add(ZomAllState.Move, _moveState);
        stateController.Add(ZomAllState.Attack, _attackState);
        stateController.Add(ZomAllState.Scream,_screamState);
        stateController.Add(ZomAllState.Stun_1, _stunState);
        stateController.Add(ZomAllState.Dead, _deadState);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _currentState = stateController[ZomAllState.Move];
        _currentState.EnterState();
    }

    protected override void OnEventDetectorDead(int obj)
    {
        base.OnEventDetectorDead(obj);
        ChangeState(ZomAllState.Stun_1);
    }

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