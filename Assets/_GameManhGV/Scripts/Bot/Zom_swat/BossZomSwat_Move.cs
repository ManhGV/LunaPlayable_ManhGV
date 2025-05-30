using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossZomSwat_Move : BaseState<BossZomSwatState>
{
    [SerializeField] private HumanMoveBase humanMoveBase;
    private WayPoint path;
    private int moveIndex;
    private bool moveDoneToAttack;

    public override void EnterState()
    {
        thisBotNetwork.ChangeAnim("Move");
        path = thisBotNetwork.wayPoint;
        moveIndex = 0;
        isDoneState = false;

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
        if (isDoneState)
            return;
        
        if (path != null)
        {
            if (moveDoneToAttack)
            {
                if (!humanMoveBase.isHaveParent)
                {
                    humanMoveBase.SetBotMove(path.AttackWayPoints[moveIndex],.9f);
                    float distance = Vector3.Distance(humanMoveBase.myTrans.position, path.AttackWayPoints[moveIndex].position);
                    if (distance < 0.1f)
                    {
                        isDoneState = true;
                    }
                }
            }
            else
            {
                if (!humanMoveBase.isHaveParent && moveIndex < path.WayPoints.Count)
                {
                    humanMoveBase.SetBotMove(path.WayPoints[moveIndex]);
                    float distance = Vector3.Distance(humanMoveBase.myTrans.position, path.WayPoints[moveIndex].position);
                    if (distance < 0.1)
                    {
                        moveIndex++;
                    }
                }

                if (moveIndex == path.WayPoints.Count)
                {
                    thisBotNetwork.SetIntAnim("MoveType", 1);
                    moveDoneToAttack = true;
                    isDoneState = true;
                }
            }
        }
    }

    public override void ExitState()
    {
    }

    public override BossZomSwatState GetNextState()
    {
        if (thisBotNetwork.IsDead)
        {
            return BossZomSwatState.Dead;
        }
        else
        {
            if (isDoneState)
            {
                return BossZomSwatState.Attack;
            }
            else
            {
                return StateKey;
            }
        }
    }
}