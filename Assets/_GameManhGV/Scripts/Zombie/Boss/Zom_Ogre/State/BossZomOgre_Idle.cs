using static GameConstants;
using UnityEngine;

public class BossZomOgre_Idle : StateBase<ZomAllState,BossNetwork>
{
    private float timer;
    public override void EnterState()
    {
        thisBotNetworks.ChangeAnim("Idle");
        timer = thisBotNetworks.BotConfigSO.coolDownAttack;
    }

    public override void UpdateState()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
            thisStateController.ChangeState(ZomAllState.Attack);
    }

    public override void ExitState()
    {
        
    }
}