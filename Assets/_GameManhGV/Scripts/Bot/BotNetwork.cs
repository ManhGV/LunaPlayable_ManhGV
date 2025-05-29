using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BotNetwork : MonoBehaviour, ITakeDamage
{
    [SerializeField] private bool isBoss;
    [SerializeField] BotConfigSO botConfigSO;
    [SerializeField] Image healthBarUI;
    [SerializeField] Transform healthBarTransform;
    [SerializeField] private int _currentHealth;
    [SerializeField] private bool isImmortal;
    [SerializeField] private bool isDead;
    public bool IsDeadExplosion;
    
    public  WayPoint wayPoint;

    [Header("Change Anim")] [SerializeField]
    private string currentAnimName;

    [SerializeField] private Animator anim;

    public BotConfigSO BotConfigSO => botConfigSO;
    
    [SerializeField] private List<GameObject> detectors = new List<GameObject>();
    
    public bool IsDead => isDead;
    public int currentHealth => _currentHealth;
    public bool IsImmortal => isImmortal;

    public Action<int> OnTakeDamage { get; set; }
    public Action<string, int> OnWeaknessTakeDamage { get; set; }

    public Action<float> OnHealthChanged { get; set; }
    public Action OnBotDead { get; set; }
    public Action<BotNetwork> OnBotNetWorkDead { get; set; }

    private Coroutine hideHealthBarCoroutine; // Tham chiếu tới Coroutine
    public Transform mainCameraTranform;
    
    
    private void Awake()
    {
        if (mainCameraTranform == null)
        {
            mainCameraTranform = Camera.main.transform;
        }
        OnBotDead+= Die;
        _currentHealth = botConfigSO.health;

        if (healthBarTransform != null)
        {
             healthBarTransform.gameObject.SetActive(false); 
        }    
        //healthBar.enabled = false; // Ẩn thanh máu khi khởi tạo
        isImmortal = false;
    }

    public void TakeDamage(DamageInfo damageInfo)
    {   
        if(isDead) return;
        
        OnTakeDamage?.Invoke(damageInfo.damage);
        
        CacularHealth(damageInfo);
        
        if(healthBarTransform != null && damageInfo.damageType != DamageType.Gas||healthBarTransform != null && isBoss && damageInfo.damageType == DamageType.Gas)
        {
            healthBarTransform.gameObject.SetActive(true);
            // Nếu đã có một Coroutine đang chờ ẩn thanh máu, hủy nó và tạo lại
            if (hideHealthBarCoroutine != null)
            {
                StopCoroutine(hideHealthBarCoroutine);
            }

            // Bắt đầu Coroutine để ẩn thanh máu sau 1 giây nếu không nhận thêm sát thương
            hideHealthBarCoroutine = StartCoroutine(HideHealthBarAfterDelay());
        }
    }
    
    
    private void CheckImmortalStatus()
    {
        if (_currentHealth <= botConfigSO.health * (botConfigSO.HealthThreshold / 100f))
        {
            isImmortal = true;
            
        }
        else
        {
            isImmortal = false;
        }
    }

    public void CacularHealth(DamageInfo damageInfo)
    {
        int damage = damageInfo.damage;
        if (isImmortal)
        {
            var damageScale = botConfigSO.GetDamageScale(damageInfo.damageType);

            if (damageScale > 0)
            {
                // Giảm damage theo phần trăm khi bất tử
                float reducedDamage = damageInfo.damage * damageScale;
                damage = Mathf.CeilToInt(reducedDamage); // Làm tròn lên 
            }


        }
        if (isImmortal && botConfigSO.isCanImmortal) return;
        _currentHealth -= damage;
        if(damageInfo.damageType == DamageType.Weekness)
        {
            OnWeaknessTakeDamage?.Invoke(damageInfo.name, damageInfo.damage);
        }
        SetHealthBar(_currentHealth);

        CheckImmortalStatus(); // Kiểm tra điều kiện bất tử
        if (_currentHealth <= 0)
        {
            isDead = true;
            OnBotDead.Invoke();
        }
    }
    public void Die()
    {
        isDead = true;
        _currentHealth = 0;
        if (healthBarTransform == null) return;
        healthBarTransform.gameObject.SetActive(false);
        OnBotNetWorkDead?.Invoke(this);
    }

    private void SetHealthBar(float currentHealth)
    {
        float healthBarValue = (currentHealth / botConfigSO.health);
        OnHealthChanged?.Invoke(healthBarValue);
        if (healthBarUI != null)
        {
            healthBarUI.fillAmount = healthBarValue;
        }  
    }

    private IEnumerator HideHealthBarAfterDelay()
    {
        if (isDead)
        {
            healthBarTransform.gameObject.SetActive(false);
        }
        // Chờ 1 giây
        yield return new WaitForSeconds(2f);

        // Ẩn thanh máu nếu bot chưa chết
        if (!isDead)
        {
            healthBarTransform.gameObject.SetActive(false);
        }
        hideHealthBarCoroutine = null;
    }

    private void Update()
    {
        NUtiliti.AlignCamera(healthBarTransform, mainCameraTranform);
    }

    public void ChangeAnim(string _name)
    {
        if (anim == null)
        {
            Debug.LogError("Null anim");
            return;
        }
        
        if (currentAnimName != _name)
        {
            anim.ResetTrigger(_name);
            currentAnimName = _name;
            anim.SetTrigger(currentAnimName);
        }
    }

    public void ActiveFalseDetectors()
    {
        foreach (GameObject VARIABLE in detectors)
        {
            VARIABLE.SetActive(false);
        }
    }
}


