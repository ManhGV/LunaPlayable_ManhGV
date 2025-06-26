using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossNetwork : ZombieBase
{
    [SerializeField] protected List<GameObject> detectors = new List<GameObject>();
    public Action<int> ActionEventDetectorDead { get; set; }
    protected override void Awake()
    {
        base.Awake();
        if(isBotActiveEqualTay)
            OnInit();
    }

    public override void TakeDamage(DamageInfo damageInfo)
    {
        if(isDead || isImmortal)
            return;
        
        OnTakeDamage?.Invoke(damageInfo);
        if(healthBarTransform != null)
        {
            CacularHealth(damageInfo);
            healthBarTransform.gameObject.SetActive(true);
            
            if (hideHealthBarCoroutine != null)
                StopCoroutine(hideHealthBarCoroutine);
        
            hideHealthBarCoroutine = StartCoroutine(IEHideHealthBarAfterDelay());
        }
    }

    public AchievementEvaluator achievementEvaluator;

    public override void BotDead()
    {
        base.BotDead();
        foreach (GameObject VARIABLE in detectors)
            VARIABLE.SetActive(false);
        SpawnBotManager.Instance.RemoveBotDead(this);
        achievementEvaluator.ResetKillData();
        achievementEvaluator.GrantMedal(4);
        GameManager.Instance.EndGame(true);
    }
    
    public virtual void SetActiveDetectors(bool _active,int _skillType)
    {
        
    }

    public void SetActiveAllDetectors(bool _active)
    {
        foreach (GameObject VARIABLE in detectors)
            VARIABLE.SetActive(_active);
    }
    
    public void OnEventDetectorDead(int _stunType)
    {
        if(isDead)
            return;
        for (int i = 0; i < detectors.Count; i++)
            detectors[i].SetActive(false);
        ActionEventDetectorDead?.Invoke(_stunType);
    }
}