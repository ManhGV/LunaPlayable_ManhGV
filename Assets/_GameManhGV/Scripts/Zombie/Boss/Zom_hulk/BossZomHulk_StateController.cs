using static GameConstants;
using UnityEngine;

[RequireComponent(typeof(BossZomHulk_Idle),typeof(BossZomHulk_Move),typeof(BossZomHulk_Jump))]
[RequireComponent(typeof(BossZomHulk_JumpPunch),typeof(BossZomHulk_Attack),typeof(BossZomHulk_Scream))]
[RequireComponent(typeof(BossZomHulk_Stun),typeof(BossZomHulk_Dead))]
public class BossZomHulk_StateController : StateControllerBase<BossZomHulk_Netword>
{
    private BossZomHulk_Idle _idleState;
    private BossZomHulk_Move _moveState;
    private BossZomHulk_Jump _jumpState;
    private BossZomHulk_JumpPunch _jumpPunchState;
    private BossZomHulk_Attack _attackState;
    private BossZomHulk_Scream _screamState;
    private BossZomHulk_Stun _stunState;
    private BossZomHulk_Dead _deadState;

    private void Awake()
    {
        _idleState = GetComponent<BossZomHulk_Idle>();
        _idleState.Initialize(ZomAllState.Idle, botNetworks,this);
        
        _moveState = GetComponent<BossZomHulk_Move>();
        _moveState.Initialize(ZomAllState.Move, botNetworks,this);
        
        _jumpState = GetComponent<BossZomHulk_Jump>();
        _jumpState.Initialize(ZomAllState.Jump, botNetworks,this);
        
        _jumpPunchState = GetComponent<BossZomHulk_JumpPunch>();
        _jumpPunchState.Initialize(ZomAllState.JumpPunch, botNetworks,this);
        
        _attackState = GetComponent<BossZomHulk_Attack>();
        _attackState.Initialize(ZomAllState.Attack, botNetworks,this);
        
        _screamState = GetComponent<BossZomHulk_Scream>();
        _screamState.Initialize(ZomAllState.Scream, botNetworks,this);
        
        _stunState = GetComponent<BossZomHulk_Stun>();
        _stunState.Initialize(ZomAllState.Stun_1, botNetworks,this);
        
        _deadState = GetComponent<BossZomHulk_Dead>();
        _deadState.Initialize(ZomAllState.Dead, botNetworks,this);
        
        stateController.Add(ZomAllState.Idle, _idleState);
        stateController.Add(ZomAllState.Move, _moveState);
        stateController.Add(ZomAllState.Jump, _jumpState);
        stateController.Add(ZomAllState.JumpPunch, _jumpPunchState);
        stateController.Add(ZomAllState.Attack, _attackState);
        stateController.Add(ZomAllState.Scream, _screamState);
        stateController.Add(ZomAllState.Stun_1, _stunState);
        stateController.Add(ZomAllState.Dead, _deadState);
    }
    
    protected override void OnEnable()
    {
        base.OnEnable();
        _currentState = stateController[ZomAllState.Move];
        _currentState.EnterState();
    }
}