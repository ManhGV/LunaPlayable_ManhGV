using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameConstants;

public class BossZomThrower_Dead : StateBase<ZomAllState, BossZomThrower_Netword>
{
    [SerializeField] private ParticleSystem vfxDead;
    [SerializeField] private GameObject _body;
    public override void EnterState()
    {
        _body.SetActive(false);
        vfxDead.Play();
    }

    public override void UpdateState()
    {
        
    }

    public override void ExitState()
    {
        
    }
}