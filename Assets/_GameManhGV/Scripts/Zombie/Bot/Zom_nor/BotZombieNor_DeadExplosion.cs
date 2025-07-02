using System.Collections;
using static GameConstants;
using UnityEngine;
// 0 = trước
// 1 = phải
// 2 = trái
// 3 = sau
public class BotZombieNor_DeadExplosion : StateBase<ZomAllState, BotNetwork>
{
    public override void EnterState()
    {
        int animType = thisBotNetworks.GetNearestDirection();
        
        thisBotNetworks.PlayAudioVoice(Random.Range(0,4),1, false);
        thisBotNetworks.ChangeAnimAndType("DeadExplosion", animType);

        StartCoroutine(IEDelayAnimAndDespawn(3f));
    }

    public override void UpdateState()
    {
        
    }

    public override void ExitState()
    {
    }
    
    private IEnumerator IEDelayAnimAndDespawn(float _timerDelay)
    {
        yield return new WaitForSeconds(_timerDelay);
        thisBotNetworks.OnDespawn();
    }
}