using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossZomSwat_Dead : StateBase<BossZomSwatState,BossZomSwatNetword>
{
    [SerializeField] private GameObject[] body;
    [SerializeField] ParticleSystem particle_Dead;
    public override void EnterState()
    {
        particle_Dead.Play();
        foreach (GameObject _body in body)
            _body.SetActive(false);
        isDoneState = false;
        thisBotNetworks.ActiveFalseDetectors();
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