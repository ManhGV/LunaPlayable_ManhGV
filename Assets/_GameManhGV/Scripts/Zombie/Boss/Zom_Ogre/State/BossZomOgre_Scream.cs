using static GameConstants;
using UnityEngine;

public class BossZomOgre_Scream : StateBase<ZomAllState,BossNetwork>
{
    public override void EnterState()
    {
        thisBotNetworks.ChangeAnim("Sceam");
    }

    public override void UpdateState()
    {
        
    }

    public override void ExitState()
    {
        
    }
}