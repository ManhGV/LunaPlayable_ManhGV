using System.Collections;
using UnityEngine;
using static GameConstants;

public class BotZombieNor_Dead : StateBase<ZomAllState, BotNetwork>
{
    [SerializeField] private Transform _posCenter;
    int animType;
    public override void EnterState()
    {
        animType = Random.Range(0, 2);
        thisBotNetworks.ChangeAnimAndType("Dead", animType);

        thisBotNetworks.PlayAudioVoice(Random.Range(0,4),1,false);
        
        if (animType == 0)
            StartCoroutine(IEDelayAnimAndDespawn(4f));
        else if (animType == 1)
            StartCoroutine(IEDelayAnimAndDespawn(5f));
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
        thisBotNetworks.OnDespawn();
    }

    public override void ExitState()
    {

    }
}