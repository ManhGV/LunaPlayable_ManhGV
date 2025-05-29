using System.Collections;
using System.Collections.Generic;
using UnityEditor.TextCore.Text;
using UnityEngine;

public class BotZombieNor_Idle : BaseState<BotZomNorState>
{
    private float timereload;
    public override void EnterState()
    {
        thisBotNetwork.ChangeAnim("Idle");
        isDoneState = false;
        timereload = thisBotNetwork.BotConfigSO.coolDownAttack;
    }

    public override void UpdateState()
    {
        if(isDoneState)
            return;
        
        timereload -= Time.deltaTime;
        if(timereload <= 0)
            isDoneState = true;
    }

    public override void ExitState()
    {
        
    }

    public override BotZomNorState GetNextState()
    {
        if (thisBotNetwork.IsDeadExplosion)
            return BotZomNorState.DeadExplosion;
        else
        {
            if (thisBotNetwork.IsDead)
            {
                return BotZomNorState.Dead;
            }
            else
            {
                if(isDoneState)
                    return BotZomNorState.Attack;
                else
                    return StateKey;
            }
        }
    }
}