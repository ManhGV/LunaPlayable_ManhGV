using static GameConstants;
using UnityEngine;

public class Bot_zom_dog_StateController : StateControllerBase<BotNetwork>
{
    private BotZombieNor_Idle _idleState;
    private Bot_zom_deadExplosion_Move _moveState;
    private BotZombieNor_Attack _attackState;
    private BotZombieNor_Dead _deadState;

    private void Awake()
    {
        _idleState = GetComponent<BotZombieNor_Idle>();
        _idleState.Initialize(ZomAllState.Idle, botNetworks, this);
        
        _moveState = GetComponent<Bot_zom_deadExplosion_Move>();
        _moveState.Initialize(ZomAllState.Move, botNetworks, this);
        
        _attackState = GetComponent<BotZombieNor_Attack>();
        _attackState.Initialize(ZomAllState.Attack, botNetworks, this);
        
        _deadState = GetComponent<BotZombieNor_Dead>();
        _deadState.Initialize(ZomAllState.Dead, botNetworks, this);
        
        stateController.Add(ZomAllState.Idle, _idleState);
        stateController.Add(ZomAllState.Move, _moveState);
        stateController.Add(ZomAllState.Attack, _attackState);
        stateController.Add(ZomAllState.Dead, _deadState);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _currentState = stateController[ZomAllState.Move];
        _currentState.EnterState();
    }
}