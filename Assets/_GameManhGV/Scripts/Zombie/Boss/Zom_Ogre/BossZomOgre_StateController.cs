using static GameConstants;
using UnityEngine;

[RequireComponent(typeof(BossZomOgre_Idle),typeof(BossZomOgre_Move), typeof(BossZomOgre_Attack))]
[RequireComponent(typeof(BossZomOgre_Stun),typeof(BossZomOgre_Dead), typeof(BossZomOgre_Scream))]

public class BossZomOgre_StateController : StateControllerBase<BossZomOgre_Network>
{
    private BossZomOgre_Idle _idleState;
    private BossZomOgre_Move _moveState;
    private BossZomOgre_Attack _attackState;
    private BossZomOgre_Stun _stunState;
    private BossZomOgre_Scream _screamState;
    private BossZomOgre_Dead _deadState;
    
    private void Awake()
    {
        _idleState = GetComponent<BossZomOgre_Idle>();
        _idleState.Initialize(ZomAllState.Idle, botNetworks,this);
        
        _moveState = GetComponent<BossZomOgre_Move>();
        _moveState.Initialize(ZomAllState.Move, botNetworks,this);
        
        _attackState = GetComponent<BossZomOgre_Attack>();
        _attackState.Initialize(ZomAllState.Attack, botNetworks,this);
        
        _stunState = GetComponent<BossZomOgre_Stun>();
        _stunState.Initialize(ZomAllState.Stun_1, botNetworks,this);
        
        _screamState = GetComponent<BossZomOgre_Scream>();
        _screamState.Initialize(ZomAllState.Scream, botNetworks,this);
        
        _deadState = GetComponent<BossZomOgre_Dead>();
        _deadState.Initialize(ZomAllState.Dead, botNetworks,this);
        
        stateController.Add(ZomAllState.Idle, _idleState);
        stateController.Add(ZomAllState.Move, _moveState);
        stateController.Add(ZomAllState.Attack, _attackState);
        stateController.Add(ZomAllState.Stun_1, _stunState);
        stateController.Add(ZomAllState.Scream, _screamState);
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
}