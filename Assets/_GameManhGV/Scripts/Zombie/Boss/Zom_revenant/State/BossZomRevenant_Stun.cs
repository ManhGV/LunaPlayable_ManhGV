using static GameConstants;
using UnityEngine;

public class BossZomRevenant_Stun : StateBase<ZomAllState,BossZomRevenant_Netword>
{
    private float timeStun;
    public override void EnterState()
    {
        thisBotNetworks.ChangeAnim("Stun");
        timeStun = Random.Range(1f, 3.5f);
        //TODO:Sang stun
    }

    public override void UpdateState()
    {
        //TODO:Hết stun thì done state => random jump,attack
        timeStun -= Time.deltaTime;
        if (timeStun <= 0)
            thisStateController.ChangeState(ZomAllState.Scream);
    }

    public override void ExitState()
    {
        
    }
}
