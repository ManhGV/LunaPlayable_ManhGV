using UnityEngine;
using static GameConstants;

public class BossZomRevenant_StateController : StateControllerBase<BossZomRevenant_Netword>
{
    //BossZomRevenant_Start _startState;
    BossZomRevenant_Idle _idle;
    BossZomRevenant_Move _moveState;
    BossZomRevenant_Attack _attackState;
    BossZomRevenant_Stun _stunState;
    BossZomRevenant_Scream _screamState;
    BossZomRevenant_Jump _jumpState;
    BossZomRevenant_Dead _deadState;
    
    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        //_startState = GetComponent<BossZomRevenant_Start>();
        //_startState.Initialize(BossZomState.Start);
        
        _idle = GetComponent<BossZomRevenant_Idle>();
        _idle.Initialize(ZomAllState.Idle);

        _moveState = GetComponent<BossZomRevenant_Move>();
        _moveState.Initialize(ZomAllState.Move);

        _stunState = GetComponent<BossZomRevenant_Stun>();
        _stunState.Initialize(ZomAllState.Stun_1);
        
        _screamState = GetComponent<BossZomRevenant_Scream>();
        _screamState.Initialize(ZomAllState.Scream);
        
        _jumpState = GetComponent<BossZomRevenant_Jump>();
        _jumpState.Initialize(ZomAllState.Jump);
        
        _attackState = GetComponent<BossZomRevenant_Attack>();
        _attackState.Initialize(ZomAllState.Attack);

        _deadState = GetComponent<BossZomRevenant_Dead>();
        _deadState.Initialize(ZomAllState.Dead);

        //stateController.Add(BossZomState.Start, _startState);
        stateController.Add(ZomAllState.Idle, _idle);
        stateController.Add(ZomAllState.Move, _moveState);
        stateController.Add(ZomAllState.Stun_1, _stunState);
        stateController.Add(ZomAllState.Scream, _screamState);
        stateController.Add(ZomAllState.Jump, _jumpState);
        stateController.Add(ZomAllState.Attack, _attackState);
        stateController.Add(ZomAllState.Dead, _deadState);
    }

    protected override void OnTakeDame(int damage)
    {
        base.OnTakeDame(damage);
        print("-[BossZomRevenant] - " + damage+" HP.");
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _currentState = stateController[ZomAllState.Idle];
        _currentState.EnterState();
    }

    protected override void OnEventDetectorDead(int obj)
    {
        base.OnEventDetectorDead(obj);
        ChangeState(ZomAllState.Stun_1);
    }
}