using static GameConstants;
using UnityEngine;

public class BossZomChainSaw_Stun : StateBase<ZomAllState, BossZomChainSaw_NetWork>
{
    private float timerEndState;
    public override void EnterState()
    {
        timerEndState = 1.6f;
        thisBotNetworks.ChangeAnimAndType("OnHit", 0);
        thisBotNetworks.PlayAudioVoice(3,.5f,false);
    }

    public override void UpdateState()
    {
        timerEndState -= Time.deltaTime;
        if (timerEndState <= 0)
        {
            if(Random.Range(0,2) == 0)
                thisStateController.ChangeState(ZomAllState.Attack);
            else
                thisStateController.ChangeState(ZomAllState.Scream);
        }
    }
    
    public override void ExitState()
    {
        
    }
}