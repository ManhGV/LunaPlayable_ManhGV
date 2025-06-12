using static GameConstants;
using UnityEngine;

public class BossZomSwat_Idle : StateBase<ZomAllState,BossZomSwatNetword>
{
    private float timereload;
    int animType;

    public override void EnterState()
    {
        animType = Random.Range(0, 2);
        thisBotNetworks.ChangeAnimAndType("Idle",animType);
        timereload = thisBotNetworks.BotConfigSO.coolDownAttack;
    }

    public override void UpdateState()
    {
        timereload -= Time.deltaTime;
        if (timereload <= 0)
            thisStateController.ChangeState(ZomAllState.Attack);
    }

    public override void ExitState()
    {
        
    }
}