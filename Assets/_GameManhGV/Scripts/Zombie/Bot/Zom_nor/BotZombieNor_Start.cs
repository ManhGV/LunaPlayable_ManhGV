using UnityEngine;

public class BotZombieNor_Start : StateBase<BotZomNorState, BotNetwork>
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
        isDoneState = true;
    }

    public override void EnterState()
    {
        thisBotNetworks.OnTakeDamage += OnTakeDamage;
        thisBotNetworks.SetAnimAndType("Start", typeStart);
        isDoneState = false;
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
        if (isDoneState)
            return; 
        
        timer -= Time.deltaTime;
        if (timer <= 0)
            isDoneState = true;
    }

    public override void ExitState()
    {
        thisBotNetworks.OnTakeDamage -= OnTakeDamage;
    }
    
    public override BotZomNorState GetNextState()
    {
        if (thisBotNetworks.IsDeadExplosion)
            return BotZomNorState.DeadExplosion;
        else
        {
            if (thisBotNetworks.IsDead)
            {
                return BotZomNorState.Dead;
            }
            else
            {
                if(isDoneState)
                    return BotZomNorState.Move;
                else
                    return StateKey;
            }
        }
    }
}