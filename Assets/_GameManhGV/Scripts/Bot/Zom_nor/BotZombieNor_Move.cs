using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotZombieNor_Move : BaseState<BotZomNorState>
{
    [SerializeField] private HumanMoveBase humanMoveBase;
    [SerializeField] private bool moveType_2;
    private WayPoint path;
    private int moveIndex;
    private int typeMove;
    private float speedTypeIndex;
    
    public override void EnterState()
    {
        if(moveType_2)
            typeMove = 2;
        else
            typeMove = Random.Range(0, 4);
        
        thisBotNetwork.SetAnimAndType("Move",typeMove);
        path = thisBotNetwork.wayPoint;
        
        if (typeMove == 0)
            speedTypeIndex = .9f;
        else if (typeMove == 1)
            speedTypeIndex = .87f;
        else if (typeMove == 2)
            speedTypeIndex = .28f;
        else
            speedTypeIndex = 3f;
        isDoneState = false;
    }

    public override void UpdateState()
    {
        if (path != null)
        {
            if (!humanMoveBase.isHaveParent && moveIndex < path.WayPoints.Count)
            {
                humanMoveBase.SetBotMove(path.WayPoints[moveIndex],speedTypeIndex);
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