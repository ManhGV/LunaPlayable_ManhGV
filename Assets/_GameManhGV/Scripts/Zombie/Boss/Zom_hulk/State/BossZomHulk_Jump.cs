using static GameConstants;
using UnityEngine;

public class BossZomHulk_Jump : StateBase<ZomAllState,BossZomHulk_Netword>
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
            {
                if(Random.Range(0,10) < 5)
                    thisStateController.ChangeState(ZomAllState.Attack);
                else
                    thisStateController.ChangeState(ZomAllState.JumpPunch);
            }
        }
        else
        {
            thisStateController.ChangeState(ZomAllState.Move);
        }
    }

    public override void ExitState()
    {
        thisBotNetworks.PlayAudioVoice(2, 1, false);
    }
}