using static GameConstants;

public class BossZomSwat_State : StateControllerBase<BossZomSwatNetword>
{
    BossZomSwat_Start _startState;
    BossZomSwat_Idle _idle;
    BossZomSwat_Move _moveState;
    BossZomSwat_Attack _attackState;
    BossZomSwat_Stun_1 _stun1State;
    BossZomSwat_Stun_2 _stun2State;
    BossZomSwat_Dead _deadState;
    
    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _startState = GetComponent<BossZomSwat_Start>();
        _startState.Initialize(ZomAllState.Start);
        
        _idle = GetComponent<BossZomSwat_Idle>();
        _idle.Initialize(ZomAllState.Idle);

        _moveState = GetComponent<BossZomSwat_Move>();
        _moveState.Initialize(ZomAllState.Move);

        _stun1State = GetComponent<BossZomSwat_Stun_1>();
        _stun1State.Initialize(ZomAllState.Stun_1);
        
        _stun2State = GetComponent<BossZomSwat_Stun_2>();
        _stun2State.Initialize(ZomAllState.Stun_2);
        
        _attackState = GetComponent<BossZomSwat_Attack>();
        _attackState.Initialize(ZomAllState.Attack);

        _deadState = GetComponent<BossZomSwat_Dead>();
        _deadState.Initialize(ZomAllState.Dead);

        stateController.Add(ZomAllState.Start,_startState);
        stateController.Add(ZomAllState.Idle, _idle);
        stateController.Add(ZomAllState.Move, _moveState);
        stateController.Add(ZomAllState.Stun_1, _stun1State);
        stateController.Add(ZomAllState.Stun_2, _stun2State);
        stateController.Add(ZomAllState.Attack, _attackState);
        stateController.Add(ZomAllState.Dead, _deadState);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _currentState = stateController[ZomAllState.Start];
        _currentState.EnterState();
    }
}