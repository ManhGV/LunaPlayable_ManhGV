using static GameConstants;
using UnityEngine;

[RequireComponent(typeof(BossZomThrower_Idle),typeof(BossZomThrower_Move),typeof(BossZomThrower_Attack))]
[RequireComponent(typeof(BossZomThrower_Jump), typeof(BossZomThrower_Stun), typeof(BossZomThrower_Dead))]
public class BossZomThrower_StateController : StateControllerBase<BossZomThrower_Netword>
{
    BossZomThrower_Idle _idleState;
    BossZomThrower_Move _moveState;
    BossZomThrower_Attack _attackState;
    BossZomThrower_Jump _jumpState;
    BossZomThrower_Stun _stunState;
    BossZomThrower_Dead _deadState;

    private void Awake()
    {
        OnInit();
    }

    private void OnInit()
    {   
        _idleState = GetComponent<BossZomThrower_Idle>();
        _idleState.Initialize(ZomAllState.Idle);
        
        _moveState = GetComponent<BossZomThrower_Move>();
        _moveState.Initialize(ZomAllState.Move);
        
        _jumpState = GetComponent<BossZomThrower_Jump>();
        _jumpState.Initialize(ZomAllState.Jump);
        
        _attackState = GetComponent<BossZomThrower_Attack>();
        _attackState.Initialize(ZomAllState.Attack);
        
        _stunState = GetComponent<BossZomThrower_Stun>();
        _stunState.Initialize(ZomAllState.Stun_1);
        
        _deadState = GetComponent<BossZomThrower_Dead>();
        _deadState.Initialize(ZomAllState.Dead);
        
        stateController.Add(ZomAllState.Idle, _idleState);
        stateController.Add(ZomAllState.Move, _moveState);
        stateController.Add(ZomAllState.Jump, _jumpState);
        stateController.Add(ZomAllState.Attack, _attackState);
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