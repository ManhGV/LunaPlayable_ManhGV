using static GameConstants;
using UnityEngine;

public class Bot_zomExplosion_HaveStart_StateController : StateControllerBase<BotNetwork>
{
    
    private Bot_zom_deadExplosion_Move _moveState;
    private Bot_zom_deadExplosion_Dead _deadState;
    private Bot_zom_deadExplosion_Start _startState;

    private void Awake()
    {
        _moveState = GetComponent<Bot_zom_deadExplosion_Move>();
        _moveState.Initialize(ZomAllState.Move, botNetworks, this);
        
        _deadState = GetComponent<Bot_zom_deadExplosion_Dead>();
        _deadState.Initialize(ZomAllState.Dead, botNetworks, this);
        
        _startState = GetComponent<Bot_zom_deadExplosion_Start>();
        _startState.Initialize(ZomAllState.Start, botNetworks, this);
        
        stateController.Add(ZomAllState.Start, _startState);
        stateController.Add(ZomAllState.Move, _moveState);
        stateController.Add(ZomAllState.Dead, _deadState);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _deadState.OnInit();
        _currentState = stateController[ZomAllState.Start];
        _currentState.EnterState();
    }
    
// #if UNITY_EDITOR
//     private void OnDrawGizmos()
//     {
//         Gizmos.color = Color.red;
//         Gizmos.DrawWireSphere(botNetworks.GetTransformCenter().position, _deadState._radiusExplosion);
//     }
// #endif
}