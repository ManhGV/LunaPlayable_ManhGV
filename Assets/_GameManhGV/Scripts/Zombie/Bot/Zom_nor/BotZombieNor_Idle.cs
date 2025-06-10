using UnityEngine;

public class BotZombieNor_Idle : StateBase<BotZomNorState, BotNetwork>
{
    private float timereload;
    public override void EnterState()
    {
        thisBotNetworks.ChangeAnim("Idle");
        isDoneState = false;
        timereload = thisBotNetworks.BotConfigSO.coolDownAttack;
    }

    public override void UpdateState()
    {
        if(isDoneState)
            return;
        
        timereload -= Time.deltaTime;
        if(timereload <= 0)
            isDoneState = true;
    }

    public override void ExitState()
    {
        
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
                    return BotZomNorState.Attack;
                else
                    return StateKey;
            }
        }
    }
}