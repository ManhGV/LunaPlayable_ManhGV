using static GameConstants;
using UnityEngine;

public class BossZomRevenant_Move : StateBase<ZomAllState, BossZomRevenant_Netword>
{
    [SerializeField] float radiusMoveRandom = 5f;
    [SerializeField] HumanMoveBase humanMoveBase;
    Vector3 _pointRandomMove;

    public override void EnterState()
    {
        thisBotNetworks.ChangeAnim("Move");
        _pointRandomMove = thisBotNetworks.GetRandomPointBehindZombie(radiusMoveRandom);
    }

    public override void UpdateState()
    {
        humanMoveBase.SetBotMove(_pointRandomMove);
        if(thisBotNetworks.CheckForardChamVaoGround())
            thisStateController.ChangeState(ZomAllState.Idle);
        
        float distance = Vector3.Distance(humanMoveBase.myTrans.position, _pointRandomMove);
        if (distance < 0.1)
            thisStateController.ChangeState(ZomAllState.Attack);
    }

    public override void ExitState()
    {
        thisBotNetworks.RotaToPlayerMain();
    }
}