using System.Collections.Generic;
using static GameConstants;
using UnityEngine;

public class Bot_zom_deadExplosion_Move : StateBase<ZomAllState, BotNetwork>
{
    [SerializeField] private HumanMoveBase humanMoveBase;
    private WayPoint path;
    private int moveIndex;
    [SerializeField] private float speed;

    [SerializeField] private bool isbotDog;
    
    public override void EnterState()
    {
        Invoke(nameof(DelayGetPath), 0.1f);
        thisBotNetworks.ChangeAnim("Move");
        moveIndex = 0;
    }

    public void DelayGetPath()
    {
        path = thisBotNetworks.GetWayPoint;
    }

    public override void UpdateState()
    {
        if (path != null)
        {
            if (!humanMoveBase.isHaveParent && moveIndex < path.WayPoints.Count)
            {
                humanMoveBase.SetBotMove(path.WayPoints[moveIndex].position,speed);
                float distance = Vector3.Distance(humanMoveBase.myTrans.position, path.WayPoints[moveIndex].position);
                if (distance < 0.1)
                    moveIndex++;
            }
            if (moveIndex >= path.WayPoints.Count)
            {
                if(isbotDog)
                    thisStateController.ChangeState(ZomAllState.Attack);
                else
                    thisStateController.ChangeState(ZomAllState.Dead);
            }
        }
            
    }

    public override void ExitState()
    {
        
    }
}