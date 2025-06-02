using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BotZomNorState
{
    Start,
    Idle,
    Move,
    Attack,
    Dead,
    DeadExplosion,
}

public class BotZombieNor : MonoBehaviour
{
    public Dictionary<BotZomNorState, BaseState<BotZomNorState>> stateController = new Dictionary<BotZomNorState, BaseState<BotZomNorState>>();

    public BaseState<BotZomNorState> _currentState;
    private bool _isTransition;
    [SerializeField] private BotNetwork botNetwork;
    [SerializeField] private bool haveStart;


    BotZombieNor_Idle _idle;
    BotZombieNor_Move _moveState;
    BotZombieNor_Attack _attackState;
    BotZombieNor_Dead _deadState;
    BotZombieNor_DeadExplosion _deadExplosionState;
    public BotZombieNor_Start _startState;


    private void Awake()
    {
        _idle = GetComponent<BotZombieNor_Idle>();
        _idle.Initialize(BotZomNorState.Idle);

        _moveState = GetComponent<BotZombieNor_Move>();
        _moveState.Initialize(BotZomNorState.Move);

        _attackState = GetComponent<BotZombieNor_Attack>();
        _attackState.Initialize(BotZomNorState.Attack);

        _deadState = GetComponent<BotZombieNor_Dead>();
        _deadState.Initialize(BotZomNorState.Dead);

        _deadExplosionState = GetComponent<BotZombieNor_DeadExplosion>();
        _deadExplosionState.Initialize(BotZomNorState.DeadExplosion);

        stateController.Add(BotZomNorState.Idle, _idle);
        stateController.Add(BotZomNorState.Move, _moveState);
        stateController.Add(BotZomNorState.Attack, _attackState);
        stateController.Add(BotZomNorState.Dead, _deadState);
        stateController.Add(BotZomNorState.DeadExplosion, _deadExplosionState);

        if (haveStart)
        {
            _startState = GetComponent<BotZombieNor_Start>();
            _startState.Initialize(BotZomNorState.Start);

            stateController.Add(BotZomNorState.Start, _startState);
        }

    }

    void OnEnable()
    {
        Init();
        Invoke(nameof(Enanle), 0.1f);
    }
    
    private void Init()
    {
        _moveState.Init();
    }

    private void Enanle()
    {
        botNetwork.OnTakeDamage += OnTakeDame;
        if (haveStart)
            _currentState = stateController[BotZomNorState.Start];
        else
            _currentState = stateController[BotZomNorState.Move];
        _currentState.EnterState();
    }

    private void OnTakeDame(int damage)
    {
        print(" [dautao] " + damage);
    }

    void OnDisable()
    {
        botNetwork.OnTakeDamage -= OnTakeDame;
    }

    void Update()
    {
        if (_currentState == null)
            return;

        BotZomNorState nextState = _currentState.GetNextState();
        if (_currentState.StateKey.Equals(nextState) && !_isTransition)
        {
            _currentState.UpdateState();
        }
        else
        {
            TransitionState(nextState);
        }
    }

    private void TransitionState(BotZomNorState tankState)
    {
        _isTransition = true;
        _currentState.ExitState();
        _currentState = stateController[tankState];
        _currentState.EnterState();
        _isTransition = false;
    }
}