using static GameConstants;
using UnityEngine;

public class BossZomSwat_Stun_2 : StateBase<ZomAllState,BossZomSwatNetword>
{
    private float timeStun;

    public override void EnterState()
    {
        thisBotNetworks.SetIntAnim("StunType",1);
        thisBotNetworks.ChangeAnim("Stun");
        timeStun = 3.20f;
    }

    public override void UpdateState()
    {
        timeStun -= Time.deltaTime;
        if (timeStun <= 0)
            thisStateController.ChangeState(ZomAllState.Attack); 
    }

    public override void ExitState()
    {
        
    }
}