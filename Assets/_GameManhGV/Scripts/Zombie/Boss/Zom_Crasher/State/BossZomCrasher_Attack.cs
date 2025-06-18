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
        _attackCoroutine = StartCoroutine(IEAttack(1)); //Random.Range(0,2)));
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
            thisBotNetworks.SetFloatAnim("animSpeedAttack", 1f);
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
            thisBotNetworks.PlayAudioVoice(1,1,false);
            yield return new WaitForSeconds(0.7f);
            thisBotNetworks.PlayAudioVoice(2,1,false);
            EventManager.Invoke(EventName.OnTakeDamagePlayer,thisBotNetworks.BotConfigSO.damage);
            yield return new WaitForSeconds(1f);
            // yield return new WaitForSeconds(1.14f);
        }else if (typeAttack == 1)
        {
            // yield return new WaitForSeconds(1.81f);
            yield return new WaitForSeconds(1.1f);
            thisBotNetworks.SetActiveDetectors(true, 1);
            thisBotNetworks.SetFloatAnim("animSpeedAttack", .27f);
            yield return new WaitForSeconds(1.55f);
            thisBotNetworks.PlayAudioVoice(3,1,false);
            yield return new WaitForSeconds(.2f);
            thisBotNetworks.SetFloatAnim("animSpeedAttack", 1);
            thisBotNetworks.SetActiveDetectors(false, 1);
            yield return new WaitForSeconds(0.3f);
            thisBotNetworks.PlayAudioVoice(4,1,false);
            EventManager.Invoke(EventName.OnTakeDamagePlayer,thisBotNetworks.BotConfigSO.damage);
            yield return new WaitForSeconds(2f);
            // yield return new WaitForSeconds(2.69f);
        }

        _attackCoroutine = null;
        _doneAttack = true;
    }
}