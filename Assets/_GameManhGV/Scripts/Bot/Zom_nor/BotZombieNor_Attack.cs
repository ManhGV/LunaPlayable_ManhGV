using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotZombieNor_Attack : BaseState<BotZomNorState>
{
    [SerializeField] private float timerAttackDefault;
    private float timerAttack;
    private Transform Mytrans;

    public override void EnterState()
    {
        thisBotNetwork.ChangeAnim("Attack");
        isDoneState = false;
        timerAttack = timerAttackDefault;
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