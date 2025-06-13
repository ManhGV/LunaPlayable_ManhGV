using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameConstants;

public class BossZomCrasher_Dead : StateBase<ZomAllState, BossNetwork>
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