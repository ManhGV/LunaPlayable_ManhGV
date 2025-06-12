using static GameConstants;
using UnityEngine;

public class BossZomRevenant_Idle : StateBase<ZomAllState,BossZomRevenant_Netword>
{
    private float timeCoolDown;
    public override void EnterState()
    {
        thisBotNetworks.PlayAnim("Idle");
        timeCoolDown = thisBotNetworks.BotConfigSO.coolDownAttack;
    }

    public override void UpdateState()
    {
        timeCoolDown -= Time.deltaTime;
        if (timeCoolDown <= 0)
            thisStateController.ChangeState(ZomAllState.Attack);
    }

    public override void ExitState()
    {
        
    }
}
