using static GameConstants;
using UnityEngine;

public class BossZomChainSaw_Hit : StateBase<ZomAllState, BossNetwork>
{
    private float timerEndState = 3.01f;
    
    public override void EnterState()
    {
        thisBotNetworks.ChangeAnim("OnHit");
    }

    public override void UpdateState()
    {
        timerEndState -= Time.deltaTime;
        if(timerEndState <= 0)
            thisStateController.ChangeState(ZomAllState.Attack);
    }

    public override void ExitState()
    {
        
    }
}