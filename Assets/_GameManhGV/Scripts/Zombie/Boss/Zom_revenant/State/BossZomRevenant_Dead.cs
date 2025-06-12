using static GameConstants;
using UnityEngine;

public class BossZomRevenant_Dead : StateBase<ZomAllState,BossZomRevenant_Netword>
{
    public override void EnterState()
    {
        thisBotNetworks.PlayAnim("Dead");
    }

    public override void UpdateState()
    {
        
    }

    public override void ExitState()
    {
        
    }
}
