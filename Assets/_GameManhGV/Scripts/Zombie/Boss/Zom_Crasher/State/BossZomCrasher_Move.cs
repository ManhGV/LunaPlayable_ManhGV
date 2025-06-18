using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameConstants;

public class BossZomCrasher_Move : StateBase<ZomAllState, BossZomCrasher_Network>
{
    [SerializeField] private HumanMoveBase humanMoveBase;
    private WayPoint path;
    private int moveIndex = 0;
    private bool moveDoneToAttack;

    public override void EnterState()
    {
        path = thisBotNetworks.GetWayPoint;
        thisBotNetworks.ChangeAnim("Move");
        if (moveDoneToAttack && path.AttackWayPoints.Count > 1)
        {
            int newIndex;
            do
            {
                newIndex = Random.Range(0, path.AttackWayPoints.Count);
            } while (newIndex == moveIndex);
            moveIndex = newIndex;
        }
    }

    public override void UpdateState()
    {
        if (path != null)
        {
            if (moveDoneToAttack)
            {
                if (!humanMoveBase.isHaveParent)
                {
                    humanMoveBase.SetBotMove(path.AttackWayPoints[moveIndex].position,1.55f);
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
                    humanMoveBase.SetBotMove(path.WayPoints[moveIndex].position,1.55f);
                    float distance = Vector3.Distance(humanMoveBase.myTrans.position, path.WayPoints[moveIndex].position);
                    if (distance < 0.1)
                        moveIndex++;
                }

                if (moveIndex >= path.WayPoints.Count)
                {
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