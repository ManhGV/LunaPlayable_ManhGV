using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossZomSwat_Stun_1 : BaseState<BossZomSwatState>
{
    private float timeStun;

    public override void EnterState()
    {
        thisBotNetwork.SetIntAnim("StunType",0);
        thisBotNetwork.ChangeAnim("Stun");
        isDoneState = false;
        timeStun = 3.12f;
    }

    public override void UpdateState()
    {
        timeStun -= Time.deltaTime;
        if (timeStun <= 0)
            isDoneState = true;
    }

    public override void ExitState()
    {
    }

    public override BossZomSwatState GetNextState()
    {
        if(thisBotNetwork.IsDead)
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