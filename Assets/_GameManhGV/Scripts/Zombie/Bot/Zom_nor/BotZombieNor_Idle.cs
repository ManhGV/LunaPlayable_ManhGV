using UnityEngine;
using static GameConstants;

public class BotZombieNor_Idle : StateBase<ZomAllState, BotNetwork>
{
    private float timereload;
    public override void EnterState()
    {
        thisBotNetworks.ChangeAnim("Idle");
        timereload = thisBotNetworks.BotConfigSO.coolDownAttack;
    }

    public override void UpdateState()
    {
        timereload -= Time.deltaTime;
        if(timereload <= 0)
            thisStateController.ChangeState(ZomAllState.Attack);
    }

    public override void ExitState()
    {
        
    }
}