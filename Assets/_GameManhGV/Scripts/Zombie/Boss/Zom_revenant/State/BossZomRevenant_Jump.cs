using static GameConstants;
using UnityEngine;

public class BossZomRevenant_Jump : StateBase<ZomAllState,BossZomRevenant_Netword>
{
    Vector3 _pointJumpEnd;
    public override void EnterState()
    {
        _pointJumpEnd = thisBotNetworks.GetWayPoint.GetRandomAttackWayPoints().position;
        thisBotNetworks.JumpToTarget(_pointJumpEnd);
        //TODO: nhảy đến vị trí cố định
    }

    public override void UpdateState()
    {
        if(!thisBotNetworks.IsJumping())
            thisStateController.ChangeState(ZomAllState.Attack);
        //TODO: đến vị trí cố định thì done State => attack
    }

    public override void ExitState()
    {
        
    }
}