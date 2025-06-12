using UnityEngine;
using static GameConstants;

public class BotZombieNor_Start : StateBase<ZomAllState, BotNetwork>
{
    [SerializeField] int typeStart; // 0: Bình thường hoặc đang ăn xác, 1: nhảy từ trên xuống
    [SerializeField] bool dontUseTimer;
    float timer;

    public void CallOnTakeDamage(int obj)
    {
        OnTakeDamage(obj);
    }
    
    private void OnTakeDamage(int obj)
    {
        thisStateController.ChangeState(ZomAllState.Move);
    }

    public override void EnterState()
    {
        thisBotNetworks.OnTakeDamage += OnTakeDamage;
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
        thisBotNetworks.OnTakeDamage -= OnTakeDamage;
    }
}