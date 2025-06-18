using System;
using UnityEngine;
using UnityEngine.PlayerLoop;
using static GameConstants;

[RequireComponent(typeof(BossZomCrasher_Idle),typeof(BossZomCrasher_Move),typeof(BossZomCrasher_Attack))]
[RequireComponent(typeof(BossZomCrasher_Stun),typeof(BossZomCrasher_Dead))]
public class BossZomCrasher_StateController : StateControllerBase<BossZomCrasher_Network>
{
    BossZomCrasher_Idle _idleState;
    BossZomCrasher_Move _moveState;
    BossZomCrasher_Attack _attackState;
    BossZomCrasher_Stun _stunState;
    BossZomCrasher_Dead _deadState;

    private void Awake()
    {
        OnInit();
    }

    private void OnInit()
    {
        _idleState = GetComponent<BossZomCrasher_Idle>();
        _idleState.Initialize(ZomAllState.Idle);

        _moveState = GetComponent<BossZomCrasher_Move>();
        _moveState.Initialize(ZomAllState.Move);

        _attackState = GetComponent<BossZomCrasher_Attack>();
        _attackState.Initialize(ZomAllState.Attack);

        _stunState = GetComponent<BossZomCrasher_Stun>();
        _stunState.Initialize(ZomAllState.Stun_1);

        _deadState = GetComponent<BossZomCrasher_Dead>();
        _deadState.Initialize(ZomAllState.Dead);

        stateController.Add(ZomAllState.Idle, _idleState);
        stateController.Add(ZomAllState.Move, _moveState);
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

    protected override void OnEventDetectorDead(int obj)
    {
        base.OnEventDetectorDead(obj);
        StunThis(0);
    }

    public void StunThis(int stunType)
    {
        _stunState.SetupStunType(stunType);
        ChangeState(ZomAllState.Stun_1);
    }
}
