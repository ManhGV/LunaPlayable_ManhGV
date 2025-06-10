using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossZomSwat_Idle : StateBase<BossZomSwatState,BossZomSwatNetword>
{
    private float timereload;
    int animType;

    public override void EnterState()
    {
        animType = Random.Range(0, 2);
        thisBotNetworks.SetAnimAndType("Idle",animType);
        isDoneState = false;
        timereload = thisBotNetworks.BotConfigSO.coolDownAttack;
    }

    public override void UpdateState()
    {
        if (isDoneState)
            return;

        timereload -= Time.deltaTime;
        if (timereload <= 0)
            isDoneState = true;
    }

    public override void ExitState()
    {
    }

    public override BossZomSwatState GetNextState()
    {
        if(thisBotNetworks.IsDead)
        {
            return BossZomSwatState.Dead;
        }
        else
        {
            if (isDoneState)
                return BossZomSwatState.Attack;
            else
                return StateKey;
        }
    }
}