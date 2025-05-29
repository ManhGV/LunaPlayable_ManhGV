using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotZombieNor_Start : BaseState<BotZomNorState>
{
    [SerializeField] float timeStart;
    float timer;

    private void OnTakeDamage(int obj)
    {
        isDoneState = true;
    }

    public override void EnterState()
    {
        thisBotNetwork.OnTakeDamage += OnTakeDamage;
        thisBotNetwork.ChangeAnim("Start");
        isDoneState = false;
        timer = timeStart;
    }

    public override void UpdateState()
    {
        if (isDoneState)
            return; 
        timer -= Time.deltaTime;
        if (timer <= 0)
            isDoneState = true;
    }

    public override void ExitState()
    {
        thisBotNetwork.OnTakeDamage -= OnTakeDamage;
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
                    return BotZomNorState.Move;
                else
                    return StateKey;
            }
        }
    }
}