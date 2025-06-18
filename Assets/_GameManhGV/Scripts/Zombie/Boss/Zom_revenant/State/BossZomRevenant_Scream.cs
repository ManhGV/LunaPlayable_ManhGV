using static GameConstants;
using UnityEngine;

public class BossZomRevenant_Scream : StateBase<ZomAllState,BossZomRevenant_Netword>
{
    private float timeRoar;
    public override void EnterState()
    {
        Invoke(nameof(PlaySound),1.1f);
        thisBotNetworks.ChangeAnim("Scream");
        timeRoar = 3.2f;
    }

    public override void UpdateState()
    {
        //TODO:Hết stun thì done state => random jump,attack
        timeRoar -= Time.deltaTime;
        if (timeRoar <= 0)
        {
            if (Random.Range(0, 50) % 2 == 0) 
                thisStateController.ChangeState(ZomAllState.Jump);
            else
                thisStateController.ChangeState(ZomAllState.Attack);
        }
    }

    public override void ExitState()
    {
        
    }
    
    public void PlaySound()=> thisBotNetworks.PlayAudioVoice(2,1,false);
}
