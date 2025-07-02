using static GameConstants;
using UnityEngine;

public class BotZombieNor_Move : StateBase<ZomAllState, BotNetwork>
{
    [SerializeField] private BotZombieNorStateController _controller;
    [SerializeField] private HumanMoveBase humanMoveBase;
    private WayPoint path;
    private int moveIndex = 0;
    private int typeMove;
    private float speedTypeIndex;
    private Vector3 pointMove;
    public bool canRandomMove;
    
    //độ chênh lêch y không lớn hơn 1 thì giữ nguyên điểm cho đi tới, nếu lớn hơn thì giữ nguyên điểm và chuyển sang nhảy
    
    public override void EnterState()
    {
        if (canRandomMove)
        {
            typeMove = Random.Range(0, 4);
            if (typeMove == 0) 
                speedTypeIndex = .9f;
            else if (typeMove == 1) 
                speedTypeIndex = .87f;
            else if (typeMove == 2) 
                speedTypeIndex = .28f;
            else 
                speedTypeIndex = 3f;
        }
        else
        {
            typeMove = 3;
            speedTypeIndex = 3f;
        }
        
        
        thisBotNetworks.ChangeAnimAndType("Move",typeMove);
        path = thisBotNetworks.GetWayPoint;
        if (moveIndex == 0)
            pointMove = path.WayPoints[moveIndex].position;
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
                humanMoveBase.SetBotMove(pointMove,speedTypeIndex);
                if (!_controller.IsGround())
                    thisStateController.ChangeState(ZomAllState.Jump);
                
                float distance = Vector3.Distance(humanMoveBase.myTrans.position, pointMove);
                if (distance < 0.1)
                {
                    moveIndex++;
                    if (moveIndex >= path.WayPoints.Count)
                    {
                        thisStateController.ChangeState(ZomAllState.Attack);
                        return;
                    }
                    
                    if (pointMove.y - path.WayPoints[moveIndex].position.y > 1f)
                    {
                        pointMove = path.WayPoints[moveIndex].position;
                        pointMove.y =path.WayPoints[moveIndex-1].position.y;
                    }
                    else
                        pointMove = path.WayPoints[moveIndex].position;
                }
            }
        }
            
    }

    public override void ExitState()
    {
        if(moveIndex>= path.WayPoints.Count)
            return;
        pointMove = path.WayPoints[moveIndex].position; //vì change nhảy
    }
}