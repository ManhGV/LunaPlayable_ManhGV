using static GameConstants;
using UnityEngine;

public class Bot_zom_deadExplosion_Start : StateBase<ZomAllState, BotNetwork>
{
    [SerializeField] private float timeStart;
    private float timer;
    
    public override void EnterState()
    {
        thisBotNetworks.ChangeAnim("Start");
        timer = timeStart;
    }

    public override void UpdateState()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
            thisStateController.ChangeState(ZomAllState.Move);
    }

    public override void ExitState()
    {
        
    }
}