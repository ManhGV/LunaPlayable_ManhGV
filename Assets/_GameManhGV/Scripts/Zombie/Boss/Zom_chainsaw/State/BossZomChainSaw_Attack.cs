using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossZomChainSaw_Attack : StateBase<GameConstants.ZomAllState,BossZomChainSaw_NetWork>
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
        thisBotNetworks.ChangeAnimAndType("Attack", _attackType);
        thisBotNetworks.PlayAudioVoice(2,1,true);
        if (_attackType == 0)
        {
            yield return new WaitForSeconds(.5f);
            thisBotNetworks.SetActiveDetectors(true,0);
            thisBotNetworks.SetFloatAnim("AttackSpeedAnim", .25f);
            yield return new WaitForSeconds(1.6f);
            thisBotNetworks.SetFloatAnim("AttackSpeedAnim", 1f);
            thisBotNetworks.SetActiveDetectors(false,0);
            yield return new WaitForSeconds(.75f);
            EventManager.Invoke(EventName.OnTakeDamagePlayer, thisBotNetworks.BotConfigSO.damage);
            yield return new WaitForSeconds(.65f);
            attackDone = true;
        }else if (_attackType ==1)
        {
            yield return new WaitForSeconds(1.1f);
            thisBotNetworks.SetActiveDetectors(true,1);
            thisBotNetworks.SetFloatAnim("AttackSpeedAnim", .3f);
            yield return new WaitForSeconds(1.5f);
            thisBotNetworks.SetFloatAnim("AttackSpeedAnim", 1f);
            thisBotNetworks.SetActiveDetectors(false,1);
            yield return new WaitForSeconds(.65f);
            EventManager.Invoke(EventName.OnTakeDamagePlayer, thisBotNetworks.BotConfigSO.damage);
            yield return new WaitForSeconds(.75f);
            attackDone = true;
            // waitStart = 1.1f;
            // speedAnim1 = .3f;
            // slomotion = .5f;
            // waitTakeDamage = .65f;
            // waitEndState = 1.4f;// tay 
        }else if (_attackType == 2)
        {
            yield return new WaitForSeconds(.45f);
            thisBotNetworks.SetActiveDetectors(true,0);
            thisBotNetworks.SetFloatAnim("AttackSpeedAnim", .3f);
            yield return new WaitForSeconds(1.5f);
            thisBotNetworks.SetFloatAnim("AttackSpeedAnim", 1f);
            thisBotNetworks.SetActiveDetectors(false,0);
            yield return new WaitForSeconds(.6f);
            EventManager.Invoke(EventName.OnTakeDamagePlayer, thisBotNetworks.BotConfigSO.damage);
            yield return new WaitForSeconds(3.1f);
            attackDone = true;
            // waitStart = .45f;
            // speedAnim1 = .3f;
            // slomotion = .5f;
            // waitTakeDamage = .6f;
            // waitEndState = 3.7f; // đầu
        }
        else
        {
            print("Không có AttackType này");
            yield return new WaitForSeconds(3.28f);
            EventManager.Invoke(EventName.OnTakeDamagePlayer, thisBotNetworks.BotConfigSO.damage);
            yield return new WaitForSeconds(5.6f);
            // waitTakeDamage = 3.28f;
            // waitEndState = 5.76f;
            attackDone = true;
        }
       
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
        if (_attackCoutine != null)
        {
            StopCoroutine(_attackCoutine);
            thisBotNetworks.SetFloatAnim("AttackSpeedAnim", 1f);
            thisBotNetworks.SetActiveAllDetectors(false);
            thisBotNetworks.StopAudioThis();
        }
    }
}