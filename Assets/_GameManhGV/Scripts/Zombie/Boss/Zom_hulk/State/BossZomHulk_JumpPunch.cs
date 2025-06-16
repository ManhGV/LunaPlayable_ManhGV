using static GameConstants;
using UnityEngine;

public class BossZomHulk_JumpPunch : StateBase<ZomAllState,BossZomHulk_Netword>
{
    Vector3 pointJumpEnd;
    private bool canJump;

    public override void EnterState()
    {
        pointJumpEnd = LocalPlayer.Instance.GetPosLocalPlayer();
        canJump = thisBotNetworks.JumpPunchToTarget(pointJumpEnd);
    }

    public override void UpdateState()
    {
        if (canJump)
        {
            if (!thisBotNetworks.IsJumping())
                thisStateController.ChangeState(ZomAllState.Jump);
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