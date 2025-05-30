using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossZomSwat_Attack : BaseState<BossZomSwatState>
{
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

    public override void EnterState()
    {
        indexSkill = Random.Range(0, dataAttackBoss._dataSkill.Length);
        isStun_1 = false;
        isStun_2 = false;
        isDoneState = false;

        #region Enble Detector Skill

        if (indexSkill == 1)
        {
            foreach (Detector VARIABLE in AttakType_1)
                VARIABLE.gameObject.SetActive(true);
            _coroutineIEDelaySkill = StartCoroutine(IEDelaySkill());
        }
        else if (indexSkill == 2)
        {
            foreach (Detector VARIABLE in AttakType_2)
                VARIABLE.gameObject.SetActive(true);
            _coroutineIEDelaySkill = StartCoroutine(IEDelaySkill());
        }

        #endregion

        thisBotNetwork.SetIntAnim("AttackType", indexSkill);
        thisBotNetwork.ChangeAnim("Attack");
        timerAttack = dataAttackBoss._dataSkill[indexSkill].timeAttack;
        thisBotNetwork.RotaToTarget();
    }

    public IEnumerator IEDelaySkill()
    {
        if (indexSkill == 1)
        {
            thisBotNetwork.SetFloatAnim("Rage_AnimSpeedScale", 1f);
            yield return new WaitForSeconds(.4f);
            thisBotNetwork.SetFloatAnim("Rage_AnimSpeedScale", .2f);
            yield return new WaitForSeconds(2.3f);
            thisBotNetwork.SetFloatAnim("Rage_AnimSpeedScale", 1f);
            foreach (Detector VARIABLE in AttakType_1)
                VARIABLE.gameObject.SetActive(false);
        }
        else if (indexSkill == 2)
        {
            thisBotNetwork.SetFloatAnim("Swing_AnimSpeedScale", 1f);
            yield return new WaitForSeconds(.9f);
            thisBotNetwork.SetFloatAnim("Swing_AnimSpeedScale", .2f);
            yield return new WaitForSeconds(1.95f);
            thisBotNetwork.SetFloatAnim("Swing_AnimSpeedScale", 1f);
            foreach (Detector VARIABLE in AttakType_2)
                VARIABLE.gameObject.SetActive(false);
        }
    }

    public override void UpdateState()
    {
        if (isDoneState)
            return;

        timerAttack -= Time.deltaTime;
        if (timerAttack <= 0)
            isDoneState = true;
    }

    public override void ExitState()
    {
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

    public override BossZomSwatState GetNextState()
    {
        if (thisBotNetwork.IsDead)
        {
            return BossZomSwatState.Dead;
        }
        else
        {
            if (isStun_1)
                return BossZomSwatState.Stun_1;
            else if (isStun_2)
                return BossZomSwatState.Stun_2;
            else
            {
                if (isDoneState)
                {
                    if (Random.Range(0, 11) % 2 == 0)
                        return BossZomSwatState.Idle;
                    else
                        return BossZomSwatState.Move;
                }
                else
                    return StateKey;
            }
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