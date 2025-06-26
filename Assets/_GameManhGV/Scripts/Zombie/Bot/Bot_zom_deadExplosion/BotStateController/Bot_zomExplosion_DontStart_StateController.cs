using System;
using System.Collections.Generic;
using static GameConstants;
using UnityEngine;

public class Bot_zomExplosion_DontStart_StateController : StateControllerBase<BotNetwork>
{
    
    private Bot_zom_deadExplosion_Move _moveState;
    private Bot_zom_deadExplosion_Dead _deadState;

    private void Awake()
    {
        _moveState = GetComponent<Bot_zom_deadExplosion_Move>();
        _moveState.Initialize(ZomAllState.Move, botNetworks, this);
        
        _deadState = GetComponent<Bot_zom_deadExplosion_Dead>();
        _deadState.Initialize(ZomAllState.Dead, botNetworks, this);
        
        stateController.Add(ZomAllState.Move, _moveState);
        stateController.Add(ZomAllState.Dead, _deadState);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _deadState.OnInit();
        _currentState = stateController[ZomAllState.Move];
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