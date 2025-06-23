using UnityEngine;
using static GameConstants;

public class BotZombieNor_Start : StateBase<ZomAllState, BotNetwork>
{
    [SerializeField] int typeStart; // 0: Bình thường hoặc đang ăn xác, 1: nhảy từ trên xuống
    [SerializeField] bool dontUseTimer;
    float timer;
    private bool canChange = true;

    public void CallOnTakeDamage()
    {
        if(!canChange)
            return;
        
        thisStateController.ChangeState(ZomAllState.Move);
    }

    public override void EnterState()
    {
        if (typeStart == 2)
            thisBotNetworks.PlayAudioVoiceLoop(5,1);
        thisBotNetworks.ChangeAnimAndType("Start", typeStart);
        if (typeStart == 1)
        {
            dontUseTimer = false;
            timer = 2.34f;
        }
        else
            timer = 5f;
    }

    public override void UpdateState()
    {
        if(dontUseTimer)
            return;
        
        timer -= Time.deltaTime;
        if (timer <= 0)
            thisStateController.ChangeState(ZomAllState.Move);
    }

    public override void ExitState()
    {
        thisBotNetworks.StopAudioThis();
        canChange = false;
    }
}