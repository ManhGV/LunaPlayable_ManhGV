using static GameConstants;
using UnityEngine;

public class BossZomHulk_Idle : StateBase<ZomAllState,BossZomHulk_Netword>
{
    private float timer;
    public override void EnterState()
    {
        thisBotNetworks.ChangeAnimAndType("Idle",Random.Range(0,2));
        timer = thisBotNetworks.BotConfigSO.coolDownAttack;
    }

    public override void UpdateState()
    {
        timer -= Time.deltaTime;
        if(timer <= 0)
            thisStateController.ChangeState(ZomAllState.Attack);
    }

    public override void ExitState()
    {
        
    }
}