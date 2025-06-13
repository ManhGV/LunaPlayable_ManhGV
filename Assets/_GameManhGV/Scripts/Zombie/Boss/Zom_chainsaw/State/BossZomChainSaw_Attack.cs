using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossZomChainSaw_Attack : StateBase<GameConstants.ZomAllState,BossNetwork>
{
    private bool canAttack;
    private Coroutine _attackCoutine;
    private bool attackDone;
    
    public override void EnterState()
    {
        attackDone = false;
        _attackCoutine = StartCoroutine(IEAttack(Random.Range(0,4)));
    }

    private IEnumerator IEAttack(int _attackType)
    {
        thisBotNetworks.RotaToPlayerMain();
        float waitTakeDamage;
        float waitEndState;
        if (_attackType == 0)
        {
            waitTakeDamage = 1.1f;
            waitEndState = 1.12f;
        }else if (_attackType ==1)
        {
            waitTakeDamage = 1.28f;
            waitEndState = 1.79f;
        }else if (_attackType == 2)
        {
            waitTakeDamage = 2.08f;
            waitEndState = 2.21f;
        }
        else
        {
            waitTakeDamage = 3.28f;
            waitEndState = 5.76f;
        }
        thisBotNetworks.ChangeAnimAndType("Attack", _attackType);
        yield return new WaitForSeconds(waitTakeDamage);
        EventManager.Invoke(EventName.OnTakeDamagePlayer, thisBotNetworks.BotConfigSO.damage);
        yield return new WaitForSeconds(waitEndState);
        attackDone = true;
    }
    
    public override void UpdateState()
    {
        if (attackDone)
        {
            if (Random.Range(0, 50) % 2 == 0)
                thisStateController.ChangeState(GameConstants.ZomAllState.Move);
            else
                thisStateController.ChangeState(GameConstants.ZomAllState.Idle);
        }
    }

    public override void ExitState()
    {
        if(_attackCoutine!=null)
            StopCoroutine(_attackCoutine);
    }
}