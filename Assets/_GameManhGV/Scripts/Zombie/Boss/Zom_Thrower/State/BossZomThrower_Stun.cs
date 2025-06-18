using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameConstants;

public class BossZomThrower_Stun : StateBase<ZomAllState, BossZomThrower_Netword>
{
    private float timer;
    public override void EnterState()
    {
        timer = 1.1f;
        thisBotNetworks.ChangeAnim("Stun");
    }

    public override void UpdateState()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            if(Random.Range(0,2)==0)
               thisStateController.ChangeState(ZomAllState.Attack);
            else
               thisStateController.ChangeState(ZomAllState.Jump);
        }
    }

    public override void ExitState()
    {
        
    }
}