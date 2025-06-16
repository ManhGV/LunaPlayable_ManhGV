using static GameConstants;
using UnityEngine;

public class BossZomOgre_Move : StateBase<ZomAllState,BossZomOgre_Network>
{
    [SerializeField] private HumanMoveBase humanMoveBase;
    private WayPoint path;
    private int moveIndex;
    private int _animType;
    
    public override void EnterState()
    {
        
        thisBotNetworks.ChangeAnim("Move");
        path = thisBotNetworks.GetWayPoint;
        if (path.AttackWayPoints.Count <= 1)
        {
            Debug.LogError("Null WayPoint AttackWayPoints");
            return;
        }
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
                humanMoveBase.SetBotMove(path.AttackWayPoints[moveIndex].position, 2.2f);
                float distance = Vector3.Distance(humanMoveBase.myTrans.position, path.AttackWayPoints[moveIndex].position);
                if (distance < 0.1f)
                    thisStateController.ChangeState(ZomAllState.Attack);
            }
        }
    }

    public override void ExitState()
    {
    }
}