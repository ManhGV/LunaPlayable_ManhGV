using static GameConstants;
using UnityEngine;

public class BossZomOgre_Scream : StateBase<ZomAllState,BossZomOgre_Network>
{
    private float timer;
    public override void EnterState()
    {
        timer = 2.7f;
        thisBotNetworks.ChangeAnim("Scream");
        Invoke(nameof(playaudio),.15f);
    }

    public override void UpdateState()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            if(Random.Range(0, 2) == 0)
            {
                thisStateController.ChangeState(ZomAllState.Move);
                return;
            }
            thisStateController.ChangeState(ZomAllState.Attack);    
        }
    }

    public override void ExitState()
    {
        
    }
    
    public void playaudio()=>
        thisBotNetworks.PlayAudioVoice(0,1,false);
}