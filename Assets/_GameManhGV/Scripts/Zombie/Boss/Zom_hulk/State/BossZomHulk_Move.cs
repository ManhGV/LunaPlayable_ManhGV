using static GameConstants;
using UnityEngine;

public class BossZomHulk_Move : StateBase<ZomAllState, BossZomHulk_Netword>
{
    [SerializeField] private HumanMoveBase humanMoveBase;
    private WayPoint path;
    private int moveIndex;
    private int _animType;
    private bool moveDoneToAttack = false;
    
    public override void EnterState()
    {
        thisBotNetworks.ChangeAnim("Move");
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
            if(moveDoneToAttack)
            {
                if (!humanMoveBase.isHaveParent)
                {
                    humanMoveBase.SetBotMove(path.AttackWayPoints[moveIndex].position, 2.9f);
                    float distance = Vector3.Distance(humanMoveBase.myTrans.position, path.AttackWayPoints[moveIndex].position);
                    if (distance < 0.1f)
                    {
                        if(Random.Range(0,10) < 5)
                            thisStateController.ChangeState(ZomAllState.Attack);
                        else
                            thisStateController.ChangeState(ZomAllState.JumpPunch);
                    }
                }
            }
            else
            {
                if (!humanMoveBase.isHaveParent && moveIndex < path.WayPoints.Count)
                {
                    humanMoveBase.SetBotMove(path.WayPoints[moveIndex].position,2.9f);
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