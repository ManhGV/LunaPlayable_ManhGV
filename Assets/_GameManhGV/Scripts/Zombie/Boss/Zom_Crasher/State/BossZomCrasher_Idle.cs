using UnityEngine;
using static GameConstants;

public class BossZomCrasher_Idle : StateBase<ZomAllState, BossNetwork>
{
    [SerializeField] private AudioClip[] listSounDead;
    [SerializeField] private float timerReload;
    
    public override void EnterState()
    {
        timerReload = thisBotNetworks.BotConfigSO.coolDownAttack;
        thisBotNetworks.ChangeAnim("Idle");
    }
    
    public override void UpdateState()
    {
        timerReload -= Time.deltaTime;
        if(timerReload <= 0)
            thisStateController.ChangeState(GameConstants.ZomAllState.Attack);
    }
    
    public override void ExitState()
    {
        
    }
}