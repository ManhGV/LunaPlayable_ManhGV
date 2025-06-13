using UnityEngine;
using static GameConstants;

public class BossZomThrower_Idle : StateBase<ZomAllState, BossZomThrower_Netword>
{
    private float timer;
    public override void EnterState()
    {
        timer = thisBotNetworks.BotConfigSO.coolDownAttack;
        thisBotNetworks.ChangeAnim("Idle");
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