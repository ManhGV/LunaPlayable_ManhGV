using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using static GameConstants;

[System.Serializable]
public class AttackSkill
{
    public int audioIndex;
    public float timeTakeDamagePlayer;
    [Tooltip("thời gian này bằng thời gian chờ takeDamage cộng thời gian này")] public float timeEndAttack;
}
    
public class BotZombieNor_Attack : StateBase<ZomAllState, BotNetwork>
{
    [SerializeField] private AttackSkill[] attackSkills;
    private int animType;
    private Coroutine _coroutineTakeDamagePlayer;
    public override void EnterState()
    {
        if (attackSkills.Length > 1)
            animType = Random.Range(0, attackSkills.Length);
        else
            animType = 0;
            
        thisBotNetworks.ChangeAnimAndType("Attack",animType);
        thisBotNetworks.RotaToPlayerMain();
        _coroutineTakeDamagePlayer = StartCoroutine(IETakeDamagePlayer(attackSkills[animType]));
    }
    
    public override void UpdateState()
    {
        
    }

    public IEnumerator IETakeDamagePlayer(AttackSkill _attackSkill)
    {
        yield return new WaitForSeconds(_attackSkill.timeTakeDamagePlayer);
        thisBotNetworks.PlayAudioVoice(_attackSkill.audioIndex,1,false);
        EventManager.Invoke(EventName.OnTakeDamagePlayer, thisBotNetworks.BotConfigSO.damage);
        yield return new WaitForSeconds(_attackSkill.timeEndAttack);
        thisStateController.ChangeState(ZomAllState.Idle);
    }

    public override void ExitState()
    {
        if(_coroutineTakeDamagePlayer != null)
            StopCoroutine(_coroutineTakeDamagePlayer);
    }
}