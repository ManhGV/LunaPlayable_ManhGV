using System.Collections;
using UnityEngine;

public class BotZombieNor_Dead : BaseState<BotZomNorState>
{
    [SerializeField] private bool canSpawnBot = false;
    [SerializeField] private Transform _posCenter;
    int animType;
    public override void EnterState()
    {
        animType = Random.Range(0, 2);
        thisBotNetwork.SetAnimAndType("Dead", animType);
        isDoneState = false;
        thisBotNetwork.ActiveFalseDetectors();

        thisBotNetwork.PlayAudioVoice(Random.Range(0,4),1);
        
        if (animType == 0)
            StartCoroutine(IEDelayAnimAndDespawn(4f));
        else if (animType == 1)
            StartCoroutine(IEDelayAnimAndDespawn(5f));

        if (canSpawnBot)
            SpawnBot();
    }

    public override void UpdateState()
    {
    }

    private IEnumerator IEDelayAnimAndDespawn(float _timerDelay)
    {
        yield return new WaitForSeconds(_timerDelay);
        if (animType == 1)
        {
            Effect explosionPoolZomNor = SimplePool.Spawn<Effect>(GameConstants.PoolType.vfx_ExplosionZombieNor, _posCenter.position, Quaternion.identity);
            explosionPoolZomNor.OnInit();
        }
        thisBotNetwork.OnDespawn();
    }

    public override void ExitState()
    {

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