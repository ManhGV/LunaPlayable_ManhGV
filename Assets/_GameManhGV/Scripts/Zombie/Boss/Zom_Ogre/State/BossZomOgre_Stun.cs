using static GameConstants;
using UnityEngine;

public class BossZomOgre_Stun : StateBase<ZomAllState, BossZomOgre_Network>
{
    float timer;

    public override void EnterState()
    {
        thisBotNetworks.ChangeAnim("Stun");
        timer = 2.4f;
    }

    public override void UpdateState()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            if (Random.Range(0, 2) == 0)
            {
                thisStateController.ChangeState(ZomAllState.Scream);
                return;
            }
            int random = Random.Range(0, 3);
            if (random == 0)
                thisStateController.ChangeState(ZomAllState.Idle);
            else if (random == 1)
                thisStateController.ChangeState(ZomAllState.Attack);
            else
                thisStateController.ChangeState(ZomAllState.Move);
        }
    }

    public override void ExitState()
    {
    }
}