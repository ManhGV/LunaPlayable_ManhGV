using static GameConstants;
using UnityEngine;

public class BossZomRevenant_Dead : StateBase<ZomAllState,BossZomRevenant_Netword>
{
    [SerializeField] ParticleSystem _particleSystem;
    [SerializeField] GameObject _body;
    public override void EnterState()
    {
        thisBotNetworks.ChangeAnim("Dead");
        _body.SetActive(false);
        _particleSystem.Play();
    }

    public override void UpdateState()
    {
        
    }

    public override void ExitState()
    {
        
    }
}
