using static GameConstants;
using UnityEngine;

public class BossZomOgre_Stun : StateBase<ZomAllState,BossNetwork>
{
    public override void EnterState()
    {
        thisBotNetworks.ChangeAnim("Stun");
    }

    public override void UpdateState()
    {
        
    }

    public override void ExitState()
    {
        
    }
}