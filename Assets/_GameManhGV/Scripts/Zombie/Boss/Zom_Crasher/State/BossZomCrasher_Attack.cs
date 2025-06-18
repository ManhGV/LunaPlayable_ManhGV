using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameConstants;

public class BossZomCrasher_Attack : StateBase<ZomAllState, BossZomCrasher_Network>
{
    Coroutine _attackCoroutine;
    bool _doneAttack;
    public override void EnterState()
    {
        _doneAttack = false;
        thisBotNetworks.RotaToPlayerMain();
        _attackCoroutine = StartCoroutine(IEAttack(Random.Range(0,2)));
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
        {
            StopCoroutine(_attackCoroutine);
            thisBotNetworks.SetActiveAllDetectors(false);
        }
    }
    
    IEnumerator IEAttack(int typeAttack)
    {
        thisBotNetworks.ChangeAnimAndType("Attack", typeAttack);
        if (typeAttack == 0)
        {
            // yield return new WaitForSeconds(1.46f);
            thisBotNetworks.SetActiveDetectors(true, 0);
            thisBotNetworks.SetFloatAnim("animSpeedAttack", .3f);
            yield return new WaitForSeconds(1.66f);
            thisBotNetworks.SetFloatAnim("animSpeedAttack", 1);
            thisBotNetworks.SetActiveDetectors(false, 0);
            yield return new WaitForSeconds(0.7f);
            EventManager.Invoke(EventName.OnTakeDamagePlayer,thisBotNetworks.BotConfigSO.damage);
            yield return new WaitForSeconds(1f);
            // yield return new WaitForSeconds(1.14f);
        }else if (typeAttack == 1)
        {
            // yield return new WaitForSeconds(1.81f);
            yield return new WaitForSeconds(1.1f);
            thisBotNetworks.SetActiveDetectors(true, 1);
            thisBotNetworks.SetFloatAnim("animSpeedAttack", .27f);
            yield return new WaitForSeconds(1.75f);
            thisBotNetworks.SetFloatAnim("animSpeedAttack", 1);
            thisBotNetworks.SetActiveDetectors(false, 1);
            yield return new WaitForSeconds(0.3f);
            EventManager.Invoke(EventName.OnTakeDamagePlayer,thisBotNetworks.BotConfigSO.damage);
            yield return new WaitForSeconds(2f);
            // yield return new WaitForSeconds(2.69f);
        }

        _attackCoroutine = null;
        _doneAttack = true;
    }
}