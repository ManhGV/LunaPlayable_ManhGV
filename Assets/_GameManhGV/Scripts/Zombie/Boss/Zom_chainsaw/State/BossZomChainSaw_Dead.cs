using System.Collections;
using static GameConstants;
using UnityEngine;

public class BossZomChainSaw_Dead : StateBase<ZomAllState, BossNetwork>
{
    [SerializeField] private ParticleSystem vfx_explorionHed;
    public GameObject _body;
    
    public override void EnterState()
    {
        vfx_explorionHed.Play();
        _body.SetActive(false);
        thisBotNetworks.ChangeAnim("Dead");
    }

    public override void UpdateState()
    {
    }
    public override void ExitState()
    {
        
    }
}