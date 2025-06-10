using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossZomSwatState
{
    Start,
    Idle,
    Move,
    Attack,
    Dead,
    Stun_1,
    Stun_2,
}

[RequireComponent(typeof(BotNetwork),typeof(HumanMoveBase))]
public class BossZomSwat : MonoBehaviour
{
    public Dictionary<BossZomSwatState, BaseState<BossZomSwatState>> stateController = new Dictionary<BossZomSwatState, BaseState<BossZomSwatState>>();

    public BaseState<BossZomSwatState> _currentState;
    private bool _isTransition;
    [SerializeField] private BotNetwork botNetwork;


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
        _startState.Initialize(BossZomSwatState.Start);
        
        _idle = GetComponent<BossZomSwat_Idle>();
        _idle.Initialize(BossZomSwatState.Idle);

        _moveState = GetComponent<BossZomSwat_Move>();
        _moveState.Initialize(BossZomSwatState.Move);

        _stun1State = GetComponent<BossZomSwat_Stun_1>();
        _stun1State.Initialize(BossZomSwatState.Stun_1);
        
        _stun2State = GetComponent<BossZomSwat_Stun_2>();
        _stun2State.Initialize(BossZomSwatState.Stun_2);
        
        _attackState = GetComponent<BossZomSwat_Attack>();
        _attackState.Initialize(BossZomSwatState.Attack);

        _deadState = GetComponent<BossZomSwat_Dead>();
        _deadState.Initialize(BossZomSwatState.Dead);

        stateController.Add(BossZomSwatState.Start, _startState);
        stateController.Add(BossZomSwatState.Idle, _idle);
        stateController.Add(BossZomSwatState.Move, _moveState);
        stateController.Add(BossZomSwatState.Stun_1, _stun1State);
        stateController.Add(BossZomSwatState.Stun_2, _stun2State);
        stateController.Add(BossZomSwatState.Attack, _attackState);
        stateController.Add(BossZomSwatState.Dead, _deadState);
    }
    
    void OnEnable()
    {
        botNetwork.OnTakeDamage += OnTakeDame;
        _currentState = stateController[BossZomSwatState.Start];
        _currentState.EnterState();
    }

    private void OnTakeDame(int damage)
    {
        print(" [Boss ne] " + damage);
    }

    void OnDisable()
    {
        botNetwork.OnTakeDamage -= OnTakeDame;
    }

    void Update()
    {
        BossZomSwatState nextState = _currentState.GetNextState();
        if (_currentState.StateKey.Equals(nextState) && !_isTransition)
        {
            _currentState.UpdateState();
        }
        else
        {
            TransitionState(nextState);
        }
    }

    public void ChangeState(BossZomSwatState tankState)
    {
        if (_currentState.StateKey.Equals(tankState) || _isTransition)
            return;

        TransitionState(tankState);
    }
    
    private void TransitionState(BossZomSwatState tankState)
    {
        _isTransition = true;
        _currentState.ExitState();
        _currentState = stateController[tankState];
        _currentState.EnterState();
        _isTransition = false;
    }
}