using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BotZombieNor_Attack : BaseState<BotZomNorState>
{
    private int animType;
    private float timerAttack;
    private Transform Mytrans;

    public override void EnterState()
    {
        animType = Random.Range(0, 2);
        thisBotNetwork.SetAnimAndType("Attack",animType);
        
        isDoneState = false;
        if(animType == 0)
            timerAttack = 1.53f;
        else
            timerAttack = .54f;
        thisBotNetwork.RotaToTarget();
    }

    public override void UpdateState()
    {
        if (isDoneState)
            return;

        timerAttack -= Time.deltaTime;
        if (timerAttack <= 0)
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
                    return BotZomNorState.Idle;
                else
                    return StateKey;
            }
        }
    }
}