using static GameConstants;
using UnityEngine;

public class BossZomSwat_Dead : StateBase<ZomAllState,BossZomSwatNetword>
{
    [SerializeField] private GameObject[] body;
    [SerializeField] ParticleSystem particle_Dead;
    public override void EnterState()
    {
        particle_Dead.Play();
        foreach (GameObject _body in body)
            _body.SetActive(false);
        thisBotNetworks.SetActiveDetectors(true,0);
    }

    public override void UpdateState()
    {
        
    }

    public override void ExitState()
    {
        
    }
}