using static GameConstants;
using UnityEngine;

public class BossZomChainSaw_Scream : StateBase<ZomAllState, BossNetwork>
{
    private float timerEndState;
    public override void EnterState()
    {
        timerEndState = 3.1f;
        thisBotNetworks.ChangeAnim("Scream");
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