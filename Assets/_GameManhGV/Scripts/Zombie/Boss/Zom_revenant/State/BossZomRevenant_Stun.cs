using static GameConstants;
using UnityEngine;

public class BossZomRevenant_Stun : StateBase<ZomAllState,BossZomRevenant_Netword>
{
    private float timeStun;
    public override void EnterState()
    {
        thisBotNetworks.PlayAnim("Stun");
        timeStun = Random.Range(1f, 3.5f);
        //TODO:Sang stun
    }

    public override void UpdateState()
    {
        //TODO:Hết stun thì done state => random jump,attack
        timeStun -= Time.deltaTime;
        if (timeStun <= 0)
        {
            if (Random.Range(0, 50) % 2 == 0) 
                thisStateController.ChangeState(ZomAllState.Jump);
            else
                thisStateController.ChangeState(ZomAllState.Attack);
        }
    }

    public override void ExitState()
    {
        
    }
}
