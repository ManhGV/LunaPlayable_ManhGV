using UnityEngine;
using static GameConstants;

public class BotZombieNor_Start : StateBase<ZomAllState, BotNetwork>
{
    [SerializeField] int typeStart; // 0: Bình thường hoặc đang ăn xác, 1: ăn xác

    public void CallOnTakeDamage()
    {
        thisStateController.ChangeState(ZomAllState.Move);
        if (typeStart == 2)
            thisBotNetworks.PlayAudioVoiceLoop(5,1);
    }

    public override void EnterState()
    {
        thisBotNetworks.ChangeAnimAndType("Start", typeStart);
    }

    public override void UpdateState()
    {
        
    }

    public override void ExitState()
    {
        thisBotNetworks.StopAudioThis();
    }
}