using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossZomSwat_Dead : BaseState<BossZomSwatState>
{
    [SerializeField] ParticleSystem particle_Dead;
    public override void EnterState()
    {
        particle_Dead.Play();
        thisBotNetwork.ChangeAnim("Dead");
        isDoneState = false;
        thisBotNetwork.ActiveFalseDetectors();
    }

    public override void UpdateState()
    {
        
    }

    public override void ExitState()
    {
        
    }

    public override BossZomSwatState GetNextState()
    {
        return StateKey;
    }
}