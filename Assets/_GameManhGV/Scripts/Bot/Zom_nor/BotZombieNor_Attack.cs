using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class BotZombieNor_Attack : BaseState<BotZomNorState>
{
    private int animType;
    private float timerAttack;
    private Coroutine _coroutineTakeDamagePlayer;
    public override void EnterState()
    {
        print(gameObject.name+ " Attack");
        animType = Random.Range(0, 2);
        thisBotNetwork.SetAnimAndType("Attack",animType);
        isDoneState = false;
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
        thisBotNetwork.RotaToTarget();
    }
    
    public override void UpdateState()
    {
        if (isDoneState)
            return;

        timerAttack -= Time.deltaTime;
        if (timerAttack <= 0)
            isDoneState = true;
    }

    public IEnumerator IETakeDamagePlayer(float _time)
    {
        yield return new WaitForSeconds(_time);
        thisBotNetwork.PlayAudioVoice(4,1);
        //print(thisBotNetwork.gameObject.name);
        EventManager.Invoke(EventName.OnTakeDamagePlayer, thisBotNetwork.BotConfigSO.damage);
    }

    public override void ExitState()
    {
        if(_coroutineTakeDamagePlayer != null)
            StopCoroutine(_coroutineTakeDamagePlayer);
    }

    public override BotZomNorState GetNextState()
    {
        if (thisBotNetwork.IsDeadExplosion)
            return BotZomNorState.DeadExplosion;
        else
        {
            if (thisBotNetwork.IsDead)
            {
                return BotZomNorState.Dead;
            }
            else
            {
                if(isDoneState)
                    return BotZomNorState.Idle;
                else
                    return StateKey;
            }
        }
    }
}