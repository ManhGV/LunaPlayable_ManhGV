using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameConstants;

public class BossZomThrower_Move : StateBase<ZomAllState, BossZomThrower_Netword>
{
    [SerializeField] private HumanMoveBase humanMoveBase;
    private WayPoint path;
    private int moveIndex;
    private bool moveDoneToAttack;

    public override void EnterState()
    {
        thisBotNetworks.ChangeAnim("Move");
        thisBotNetworks.PlayAudioVoiceLoop(1,1);
        path = thisBotNetworks.GetWayPoint;
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
                    humanMoveBase.SetBotMove(path.AttackWayPoints[moveIndex].position,4);
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
                    humanMoveBase.SetBotMove(path.WayPoints[moveIndex].position,4);
                    float distance = Vector3.Distance(humanMoveBase.myTrans.position, path.WayPoints[moveIndex].position);
                    if (distance < 0.1)
                    {
                        moveIndex++;
                    }
                }

                if (moveIndex == path.WayPoints.Count)
                {
                    moveDoneToAttack = true;
                    thisStateController.ChangeState(ZomAllState.Attack);
                }
            }
        }
    }

    public override void ExitState()
    {
        thisBotNetworks.StopAudioThis();
    }
}
