using static GameConstants;
using UnityEngine;

public class BossZomRevenant_Jump : StateBase<ZomAllState,BossZomRevenant_Netword>
{
    private bool canJump;
    Vector3 _pointJumpEnd;
    public override void EnterState()
    {
        if (Random.Range(0, 10) % 2 == 0) 
            _pointJumpEnd = thisBotNetworks.GetWayPoint.GetRandomWayPoint().position;
        else
            _pointJumpEnd = thisBotNetworks.GetWayPoint.GetRandomAttackWayPoints().position;
        
        canJump = thisBotNetworks.JumpToTarget(_pointJumpEnd);
        while (!canJump)
        {
            if (Random.Range(0, 10) % 2 == 0) 
                _pointJumpEnd = thisBotNetworks.GetWayPoint.GetRandomWayPoint().position;
            else
                _pointJumpEnd = thisBotNetworks.GetWayPoint.GetRandomAttackWayPoints().position;
            canJump = thisBotNetworks.JumpToTarget(_pointJumpEnd);
        }
    }

    public override void UpdateState()
    {
        if (!thisBotNetworks.IsJumping())
        {
            // if(thisBotNetworks.TF.position.y<=.5f)
            //     thisStateController.ChangeState(ZomAllState.Move);
            // else
                thisStateController.ChangeState(ZomAllState.Attack);
        }
    }

    public override void ExitState()
    {
        
    }
}