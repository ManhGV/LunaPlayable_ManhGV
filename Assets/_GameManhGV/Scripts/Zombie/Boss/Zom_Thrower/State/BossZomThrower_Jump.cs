using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameConstants;

public class BossZomThrower_Jump : StateBase<ZomAllState, BossZomThrower_Netword>
{
    Vector3 pointJumpEnd;
    private bool canJump;

    public override void EnterState()
    {
        pointJumpEnd = thisBotNetworks.GetWayPoint.GetRandomAttackWayPoints().position;
        canJump = thisBotNetworks.JumpToTarget(pointJumpEnd);
    }

    public override void UpdateState()
    {
        if (canJump)
        {
            if (!thisBotNetworks.IsJumping())
                thisStateController.ChangeState(ZomAllState.Attack);
        }
        else
        {
            thisStateController.ChangeState(ZomAllState.Move);
        }
    }

    public override void ExitState()
    {
        
    }
}