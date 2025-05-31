using UnityEngine;

public class BotZombieNor_Start : BaseState<BotZomNorState>
{
    [SerializeField] int typeStart; // 0: Bình thường hoặc đang ăn xác, 1: nhảy từ trên xuống
    [SerializeField] bool dontUseTimer;
    float timer;

    private void OnTakeDamage(int obj)
    {
        isDoneState = true;
    }

    public override void EnterState()
    {
        thisBotNetwork.OnTakeDamage += OnTakeDamage;
        thisBotNetwork.SetAnimAndType("Start", typeStart);
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
        thisBotNetwork.OnTakeDamage -= OnTakeDamage;
    }
    
    public override BotZomNorState GetNextState()
    {
        if (thisBotNetwork.IsDeadExplosion)
            return BotZomNorState.DeadExplosion;
        else
        {
            if (thisBotNetwork.IsDead)
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