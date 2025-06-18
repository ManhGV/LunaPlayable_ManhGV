using UnityEngine;
using static GameConstants;

public class BossZomCrasher_Stun : StateBase<ZomAllState, BossZomCrasher_Network>
{
    float timerStun;
    int stunType;
    public void SetupStunType(int type) => stunType = type; //0: Stun Attack, 1: Stun Idle
    public override void EnterState()
    {
        timerStun = .4f;
        thisBotNetworks.ChangeAnimAndType("Stun", stunType);
        thisBotNetworks.PlayAudioVoice(5,1,false);
    }

    public override void UpdateState()
    {
        timerStun -= Time.deltaTime;
        if (timerStun <= 0)
            thisStateController.ChangeState(ZomAllState.Attack);
    }

    public override void ExitState()
    {
        
    }
}