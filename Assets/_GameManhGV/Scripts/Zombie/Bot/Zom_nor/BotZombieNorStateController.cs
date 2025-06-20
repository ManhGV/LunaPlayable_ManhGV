using UnityEngine;
using static GameConstants;

public class BotZombieNorStateController : StateControllerBase<BotNetwork>
{
    public bool _dontDelay;
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
        _idle.Initialize(ZomAllState.Idle, botNetworks, this);

        _moveState = GetComponent<BotZombieNor_Move>();
        _moveState.Initialize(ZomAllState.Move, botNetworks, this);

        _attackState = GetComponent<BotZombieNor_Attack>();
        _attackState.Initialize(ZomAllState.Attack, botNetworks, this);

        _deadState = GetComponent<BotZombieNor_Dead>();
        _deadState.Initialize(ZomAllState.Dead, botNetworks, this);

        _deadExplosionState = GetComponent<BotZombieNor_DeadExplosion>();
        _deadExplosionState.Initialize(ZomAllState.DeadExplosion, botNetworks, this);

        stateController.Add(ZomAllState.Idle, _idle);
        stateController.Add(ZomAllState.Move, _moveState);
        stateController.Add(ZomAllState.Attack, _attackState);
        stateController.Add(ZomAllState.Dead, _deadState);
        stateController.Add(ZomAllState.DeadExplosion, _deadExplosionState);

        if (haveStart)
        {
            _startState = GetComponent<BotZombieNor_Start>();
            _startState.Initialize(ZomAllState.Start, botNetworks, this);

            stateController.Add(ZomAllState.Start, _startState);
        }

    }

    protected override void OnEnable()
    {
        base.OnEnable();
        botNetworks.ZombieDeadExplosion += ZombieDeadExplosion;
        if (_dontDelay)
        {
            Enanle();
        }else
            Invoke(nameof(Enanle), 0.1f);
    }

    protected override void ZombieDead(bool isDead)
    {
        if (!canDead)
            return;
        canDead = false;
        ChangeState(ZomAllState.Dead);
    }

    private void ZombieDeadExplosion(bool obj)
    {
        if (!canDead)
            return;
        canDead = false;
        ChangeState(ZomAllState.DeadExplosion);
    }

    protected override void ResetState()
    {
        botNetworks.ZombieDeadExplosion -= ZombieDeadExplosion;
        base.ResetState();
        _moveState.Init();
    }

    private void Enanle()
    {
        if (haveStart)
            _currentState = stateController[ZomAllState.Start];
        else
            _currentState = stateController[ZomAllState.Move];
        _currentState.EnterState();
    }

    protected override void OnTakeDame(int damage)
    {
        base.OnTakeDame(damage);
        if(!haveStart)
            return;
        if(_startState.StateKey == ZomAllState.Start)
            _startState.CallOnTakeDamage();
    }
}