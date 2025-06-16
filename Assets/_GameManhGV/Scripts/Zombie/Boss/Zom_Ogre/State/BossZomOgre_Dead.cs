using static GameConstants;
using UnityEngine;

public class BossZomOgre_Dead : StateBase<ZomAllState,BossZomOgre_Network>
{
    public override void EnterState()
    {
        thisBotNetworks.ChangeAnim("Dead");
    }

    public override void UpdateState()
    {
        
    }

    public override void ExitState()
    {
        
    }
}