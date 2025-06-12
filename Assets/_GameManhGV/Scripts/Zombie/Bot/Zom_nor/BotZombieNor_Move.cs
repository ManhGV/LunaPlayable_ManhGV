using static GameConstants;
using UnityEngine;

public class BotZombieNor_Move : StateBase<ZomAllState, BotNetwork>
{
    [SerializeField] private HumanMoveBase humanMoveBase;
    [SerializeField] private bool moveType_2;
    private WayPoint path;
    private int moveIndex = 0;
    private int typeMove;
    private float speedTypeIndex;
    
    public override void EnterState()
    {
        if(moveType_2)
            typeMove = 2;
        else
            typeMove = Random.Range(0, 4);
        
        if (typeMove == 0)
            speedTypeIndex = .9f;
        else if (typeMove == 1)
            speedTypeIndex = .87f;
        else if (typeMove == 2)
            speedTypeIndex = .28f;
        else
            speedTypeIndex = 3f;
        
        path = thisBotNetworks.GetWayPoint;
        thisBotNetworks.SetAnimAndType("Move",typeMove);
    }

    public void Init()
    {
        moveIndex = 0;
    }

    public override void UpdateState()
    {
        if (path != null)
        {
            if (!humanMoveBase.isHaveParent && moveIndex < path.WayPoints.Count)
            {
                humanMoveBase.SetBotMove(path.WayPoints[moveIndex].position,speedTypeIndex);
                float distance = Vector3.Distance(humanMoveBase.myTrans.position, path.WayPoints[moveIndex].position);
                if (distance < 0.1)
                {
                    moveIndex++;
                }
            }
            if (moveIndex == path.WayPoints.Count)
            {
                thisStateController.ChangeState(ZomAllState.Attack);
            }
        }
    }

    public override void ExitState()
    {
        
    }
}