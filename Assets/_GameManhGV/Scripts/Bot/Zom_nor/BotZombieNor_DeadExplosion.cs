using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 0 = trước
// 1 = phải
// 2 = trái
// 3 = sau
public class BotZombieNor_DeadExplosion : BaseState<BotZomNorState>
{
    [SerializeField] private bool canSpawnBot = false;
    public override void EnterState()
    {
        isDoneState = false;
        int animType = thisBotNetwork.GetNearestDirection();
        
        thisBotNetwork.PlayAudioVoice(Random.Range(0,4),1);
        thisBotNetwork.SetAnimAndType("DeadExplosion", animType);
        
        if(canSpawnBot)
            SpawnBot();

        StartCoroutine(IEDelayAnimAndDespawn(3f));
        thisBotNetwork.ActiveFalseDetectors();
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
        thisBotNetwork.OnDespawn();
    }
    public override BotZomNorState GetNextState()
    {
        return StateKey;
    }
    
    public void SpawnBot()
    {
        SpawnBotManager.Instance.SpawnBot();
    }
}