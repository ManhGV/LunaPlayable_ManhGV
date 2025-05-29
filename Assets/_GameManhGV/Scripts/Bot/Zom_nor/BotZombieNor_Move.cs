using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotZombieNor_Move : BaseState<BotZomNorState>
{
    [SerializeField] private HumanMoveBase humanMoveBase;
    private WayPoint path;
    protected int moveIndex;
    
    public override void EnterState()
    {
        thisBotNetwork.ChangeAnim("Move");
        path = thisBotNetwork.wayPoint;
        moveIndex = 0;
        isDoneState = false;
    }

    public override void UpdateState()
    {
        if (path != null)
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
                isDoneState = true;
            }
        }
    }

    public override void ExitState()
    {
        
    }

    public override BotZomNorState GetNextState()
    {
        if (thisBotNetwork.IsDeadExplosion)
            return BotZomNorState.DeadExplosion;
        else
        {
            if (thisBotNetwork.IsDead)
            {
                return BotZomNorState.Dead;
            }
            else
            {
                if (isDoneState)
                {
                    return BotZomNorState.Attack;
                }
                else {
                    return StateKey;
                }
            }
        }
    }
}