using System.Collections;
using UnityEngine;
using static GameConstants;

public class BossZomSwat_Attack : StateBase<ZomAllState, BossZomSwatNetword>
{
    [SerializeField] EventAnim_BossZomSwat eventAnim;
    [SerializeField] private DataAttackBoss dataAttackBoss;
    private float timerAttack;
    private int indexSkill;

    //Stun
    [Tooltip("Attack Rage")] [SerializeField]
    private Detector[] AttakType_1;

    [Tooltip("Attack Swing")] [SerializeField]
    private Detector[] AttakType_2;

    private bool isStun_1;
    private bool isStun_2;
    private Coroutine _coroutineIEDelaySkill;
    private Coroutine _coroutineIEDelayEventAnim;

    public override void EnterState()
    {
        indexSkill = Random.Range(0, dataAttackBoss._dataSkill.Length);
        isStun_1 = false;
        isStun_2 = false;

        #region Enble Detector Skill

        if (indexSkill == 0)
        {
        }
        else if (indexSkill == 1)
        {
            foreach (Detector VARIABLE in AttakType_1)
                VARIABLE.gameObject.SetActive(true);
            
        }
        else if (indexSkill == 2)
        {
            foreach (Detector VARIABLE in AttakType_2)
                VARIABLE.gameObject.SetActive(true);
        }
        _coroutineIEDelaySkill = StartCoroutine(IEDelaySkill());
        _coroutineIEDelaySkill = StartCoroutine(IEDelayAnimEvent());

        #endregion

        thisBotNetworks.SetIntAnim("AttackType", indexSkill);
        thisBotNetworks.ChangeAnim("Attack");
        timerAttack = dataAttackBoss._dataSkill[indexSkill].timeAttack;
        thisBotNetworks.RotaToPlayerMain();
    }

    public IEnumerator IEDelayAnimEvent()   
    {
        if (indexSkill == 0)
        {
            yield return new WaitForSeconds(.9f);
            thisBotNetworks.PlayAudioVoice(5, 1,false);
            thisBotNetworks.PlayAudioVoice(4, 1,false);
            eventAnim.ShakeCamera();
            eventAnim.TakeDamagePlayer();
        }
        else if (indexSkill == 1)
        {
            yield return new WaitForSeconds(2.7f);//Chờ .26f
            thisBotNetworks.PlayAudioVoice(2, 1,false);
            yield return new WaitForSeconds(0.1f); //Tổng .29f
            eventAnim.TakeDamagePlayer();
            eventAnim.ShakeCamera();
            yield return new WaitForSeconds(0.796f); //Tổng 1.23f
            thisBotNetworks.PlayAudioVoice(2, 1,false);
            yield return new WaitForSeconds(0.07f); //Tổng 1.25f
            eventAnim.TakeDamagePlayer();
            eventAnim.ShakeCamera();
            yield return new WaitForSeconds(1.33f); //Tổng 3.05f
            eventAnim.ShakeCamera();
            thisBotNetworks.PlayAudioVoice(3, 1,false);
        }
        else if (indexSkill == 2)
        {
            yield return new WaitForSeconds(2.569f); //1.07
            thisBotNetworks.PlayAudioVoice(5, 1,false);
            yield return new WaitForSeconds(0.191f); //1.09
            thisBotNetworks.PlayAudioVoice(8, 1,false);
            eventAnim.ShakeCamera();
            eventAnim.TakeDamagePlayer();
        }
    }
    public IEnumerator IEDelaySkill()
    {
        if (indexSkill == 1)
        {
            thisBotNetworks.SetFloatAnim("Rage_AnimSpeedScale", 1f);
            yield return new WaitForSeconds(.4f);
            thisBotNetworks.SetFloatAnim("Rage_AnimSpeedScale", .2f);
            yield return new WaitForSeconds(2.3f);
            thisBotNetworks.SetFloatAnim("Rage_AnimSpeedScale", 1f);
            foreach (Detector VARIABLE in AttakType_1)
                VARIABLE.gameObject.SetActive(false);
        }
        else if (indexSkill == 2)
        {
            thisBotNetworks.SetFloatAnim("Swing_AnimSpeedScale", 1f);
            yield return new WaitForSeconds(.9f);
            thisBotNetworks.SetFloatAnim("Swing_AnimSpeedScale", .2f);
            yield return new WaitForSeconds(1.95f);
            thisBotNetworks.SetFloatAnim("Swing_AnimSpeedScale", 1f);
            foreach (Detector VARIABLE in AttakType_2)
                VARIABLE.gameObject.SetActive(false);
        }
    }

    public override void UpdateState()
    {
        timerAttack -= Time.deltaTime;
        if (timerAttack <= 0)
        {
            if (Random.Range(0, 11) % 2 == 0)
                thisStateController.ChangeState(ZomAllState.Idle);
            else
                thisStateController.ChangeState(ZomAllState.Move);
        }
        else
        {
            if (isStun_1)
                thisStateController.ChangeState(ZomAllState.Stun_1);
            else if (isStun_2)
                thisStateController.ChangeState(ZomAllState.Stun_2);
        }
    }

    public override void ExitState()
    {
        if(_coroutineIEDelayEventAnim != null)
            StopCoroutine(_coroutineIEDelayEventAnim);
        if (_coroutineIEDelaySkill != null)
        {
            StopCoroutine(_coroutineIEDelaySkill);
            if (indexSkill == 1)
                foreach (Detector VARIABLE in AttakType_1)
                    VARIABLE.gameObject.SetActive(false);
            else if (indexSkill == 2)
                foreach (Detector VARIABLE in AttakType_2)
                    VARIABLE.gameObject.SetActive(false);
        }
    }

    public void Stun()
    {
        if (indexSkill == 1)
        {
            isStun_1 = true;
        }
        else if (indexSkill == 2)
        {
            isStun_2 = true;
        }
        else
        {
            Debug.LogError("BossZomSwat_Attack: Stun() - Invalid indexSkill: " + indexSkill);
        }
    }
}