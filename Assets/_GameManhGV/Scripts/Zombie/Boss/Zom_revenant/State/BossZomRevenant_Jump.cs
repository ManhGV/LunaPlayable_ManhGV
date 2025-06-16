using static GameConstants;
using UnityEngine;

public class BossZomRevenant_Jump : StateBase<ZomAllState,BossZomRevenant_Netword>
{
    private bool canJump;
    Vector3 _pointJumpEnd;
    public override void EnterState()
    {
        _pointJumpEnd = thisBotNetworks.GetWayPoint.GetRandomAttackWayPoints().position;
        canJump = thisBotNetworks.JumpToTarget(_pointJumpEnd);
    }

    public override void UpdateState()
    {
        if (canJump)
        {
            if(!thisBotNetworks.IsJumping())
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