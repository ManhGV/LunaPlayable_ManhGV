using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotZombieNor_DeadExplosion : BaseState<BotZomNorState>
{
    public override void EnterState()
    {
        isDoneState = false;
    }

    public override void UpdateState()
    {
        
    }

    public override void ExitState()
    {
        
    }

    public override BotZomNorState GetNextState()
    {
        return StateKey;
    }
}