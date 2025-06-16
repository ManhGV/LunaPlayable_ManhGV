using static GameConstants;
using UnityEngine;

public class BossZomHulk_Move : StateBase<ZomAllState, BossZomHulk_Netword>
{
    [SerializeField] private HumanMoveBase humanMoveBase;
    private WayPoint path;
    private int moveIndex;
    private int _animType;
    
    public override void EnterState()
    {
        thisBotNetworks.ChangeAnim("Move");
        path = thisBotNetworks.GetWayPoint;
        int _random;
        do
        {
            _random = Random.Range(0, path.AttackWayPoints.Count);
        } while (_random == moveIndex);
        moveIndex = _random;
    }

    public override void UpdateState()
    {
        if (path != null)
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
    }

    public override void ExitState()
    {
    }
}