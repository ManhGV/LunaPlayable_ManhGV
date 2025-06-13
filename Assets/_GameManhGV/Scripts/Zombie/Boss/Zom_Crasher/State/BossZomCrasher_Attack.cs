using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameConstants;

public class BossZomCrasher_Attack : StateBase<ZomAllState, BossNetwork>
{
    Coroutine _attackCoroutine;
    bool _doneAttack;
    public override void EnterState()
    {
        _doneAttack = false;
        thisBotNetworks.RotaToPlayerMain();
        _attackCoroutine = StartCoroutine(IEAttack(1));
    }

    public override void UpdateState()
    {
        if (_doneAttack)
        {
            if (Random.Range(0, 10) % 2 == 0) 
                thisStateController.ChangeState(ZomAllState.Idle);
            else
                thisStateController.ChangeState(ZomAllState.Move);
        }
    }

    public override void ExitState()
    {
        if (_attackCoroutine != null) 
            StopCoroutine(_attackCoroutine);
    }

    IEnumerator IEAttack(int typeAttack)
    {
        thisBotNetworks.ChangeAnimAndType("Attack", typeAttack);
        if (typeAttack == 0)
        {
            yield return new WaitForSeconds(1.46f);
            EventManager.Invoke(EventName.OnTakeDamagePlayer,thisBotNetworks.BotConfigSO.damage);
            yield return new WaitForSeconds(1.14f);
        }else if (typeAttack == 1)
        {
            yield return new WaitForSeconds(1.81f);
            EventManager.Invoke(EventName.OnTakeDamagePlayer,thisBotNetworks.BotConfigSO.damage);
            yield return new WaitForSeconds(2.69f);
        }

        _doneAttack = true;
    }
}