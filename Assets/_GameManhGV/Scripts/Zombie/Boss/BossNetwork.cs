using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossNetwork : ZombieBase
{
    [SerializeField] private List<GameObject> detectors = new List<GameObject>();
    protected override void Awake()
    {
        base.Awake();
        if(isBotActiveEqualTay)
            OnInit();
    }

    public override void TakeDamage(DamageInfo damageInfo)
    {
        base.TakeDamage(damageInfo);
        
        if(healthBarTransform != null)
        {
            CacularHealth(damageInfo);
            healthBarTransform.gameObject.SetActive(true);
            
            if (hideHealthBarCoroutine != null)
                StopCoroutine(hideHealthBarCoroutine);
        
            hideHealthBarCoroutine = StartCoroutine(IEHideHealthBarAfterDelay());
        }
    }

    public override void BotDead()
    {
        base.BotDead();
        //SpawnBotManager.Instance.RemoveBotDead(this);
        AchievementEvaluator.Instance.OnBotKilled(1.8f,false);
        GameManager.Instance.EndGame(true);
    }
    
    public void ActiveFalseDetectors()
    {
        foreach (GameObject VARIABLE in detectors)
            VARIABLE.SetActive(false);
    }
}