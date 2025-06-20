using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using static GameConstants;
public class BotZombieNor_Attack : StateBase<ZomAllState, BotNetwork>
{
    private int animType;
    private float timerAttack;
    private Coroutine _coroutineTakeDamagePlayer;
    public override void EnterState()
    {
        animType = Random.Range(0, 2);
        thisBotNetworks.ChangeAnimAndType("Attack",animType);
        thisBotNetworks.RotaToPlayerMain();
        if (animType == 0)
        {
            timerAttack = 1.53f;
            _coroutineTakeDamagePlayer = StartCoroutine(IETakeDamagePlayer(0.55f));
        }
        else
        {
            timerAttack = .54f;
            _coroutineTakeDamagePlayer = StartCoroutine(IETakeDamagePlayer( 0.35f));
        }
    }
    
    public override void UpdateState()
    {
        timerAttack -= Time.deltaTime;
        if (timerAttack <= 0)
            thisStateController.ChangeState(ZomAllState.Idle);
    }

    public IEnumerator IETakeDamagePlayer(float _time)
    {
        yield return new WaitForSeconds(_time);
        thisBotNetworks.PlayAudioVoice(4,1,false);
        //print(thisBotNetwork.gameObject.name);
        EventManager.Invoke(EventName.OnTakeDamagePlayer, thisBotNetworks.BotConfigSO.damage);
    }

    public override void ExitState()
    {
        if(_coroutineTakeDamagePlayer != null)
            StopCoroutine(_coroutineTakeDamagePlayer);
    }
}