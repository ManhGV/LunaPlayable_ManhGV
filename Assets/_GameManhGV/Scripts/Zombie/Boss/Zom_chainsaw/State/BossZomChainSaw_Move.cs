using static GameConstants;
using UnityEngine;

public class BossZomChainSaw_Move : StateBase<ZomAllState,BossNetwork>
{
    [SerializeField] private HumanMoveBase humanMoveBase;
    private WayPoint path;
    private int moveIndex;
    private bool moveDoneToAttack;

    public override void EnterState()
    {
        path = thisBotNetworks.GetWayPoint;
        moveIndex = 0;

        if (moveDoneToAttack && path.AttackWayPoints.Count > 1)
        {
            int newIndex;
            do
            {
                newIndex = Random.Range(0, path.AttackWayPoints.Count);
            } while (newIndex == moveIndex);
            moveIndex = newIndex;
            thisBotNetworks.ChangeAnimAndType("Move", 1);
        }else
            thisBotNetworks.ChangeAnimAndType("Move", 0);
    }
    
    public override void UpdateState()
    {
        if (path != null)
        {
            if (moveDoneToAttack)
            {
                if (!humanMoveBase.isHaveParent)
                {
                    humanMoveBase.SetBotMove(path.AttackWayPoints[moveIndex].position,1.4f);
                    float distance = Vector3.Distance(humanMoveBase.myTrans.position, path.AttackWayPoints[moveIndex].position);
                    if (distance < 0.1f)
                    {
                        thisStateController.ChangeState(ZomAllState.Attack);
                    }
                }
            }
            else
            {
                if (!humanMoveBase.isHaveParent && moveIndex < path.WayPoints.Count)
                {
                    humanMoveBase.SetBotMove(path.WayPoints[moveIndex].position,3.3f);
                    float distance = Vector3.Distance(humanMoveBase.myTrans.position, path.WayPoints[moveIndex].position);
                    if (distance < 0.1)
                    {
                        moveIndex++;
                    }
                }

                if (moveIndex == path.WayPoints.Count)
                {
                    thisBotNetworks.ChangeAnimAndType("Move", 1);
                    moveDoneToAttack = true;
                    thisStateController.ChangeState(ZomAllState.Attack);
                }
            }
        }
    }

    public override void ExitState()
    {
    }
}