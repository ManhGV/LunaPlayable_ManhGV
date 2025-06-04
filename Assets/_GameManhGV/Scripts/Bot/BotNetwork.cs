using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BotNetwork : GameUnit, ITakeDamage
{
    [Header("Bot Audio")]
    [SerializeField] AudioSource _audioSourceVoice;
    [SerializeField] AudioClip[] _listSoundBotVoice;
    
    [Header("Bắn rocket 1 lần ngã")]
    public bool canExplosionArmor;
    [SerializeField] BallisticArmor[] arrBallisticArmor;
    [Space(10)]
    
    public bool isBotActiveEqualTay;
    public bool isBotTutorial;
    public bool isBoss;
    public AudioSource _audioSource;
    [SerializeField] private BotConfigSO botConfigSO;
    [Header("Health Bar")]
    [SerializeField] private Image healthBarUI;
    [SerializeField] private Transform healthBarTransform;
    [SerializeField] private int _currentHealth;
    [SerializeField] private bool isImmortal;
    [SerializeField] private bool isDead;
    
    [Header("Dead Explosion")]
    public bool IsDeadExplosion;
    [HideInInspector] public Vector3 _posDamageGas;
    
    [Header("Point To Move")]
    public  WayPoint wayPoint;

    [Header("Change Anim")]
    [SerializeField] private string currentAnimName;
    [SerializeField] private Animator anim;
    [SerializeField] private List<GameObject> detectors = new List<GameObject>();
    public BotConfigSO BotConfigSO => botConfigSO;
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

    public void Init(WayPoint _wayPoint)
    {
        wayPoint = _wayPoint;
        transform.position = _wayPoint.WayPoints[0].transform.position;
        _currentHealth = botConfigSO.health;
        isImmortal = false;
        isDead = false;
        IsDeadExplosion = false;
        OnBotDead += Die;
    }
    
    private void Awake()
    {
        if (mainCameraTranform == null)
        {
            mainCameraTranform = Camera.main.transform;
        }

        if (healthBarTransform != null)
        {
             healthBarTransform.gameObject.SetActive(false); 
        }

        if (isBotActiveEqualTay)
        {
            _currentHealth = botConfigSO.health;
            isImmortal = false;
            isDead = false;
            IsDeadExplosion = false;
        }
        //healthBar.enabled = false; // Ẩn thanh máu khi khởi tạo
        if(isBotActiveEqualTay)
            OnBotDead += Die;
    }

    public void TakeDamage(DamageInfo damageInfo)
    {   
        if(isDead) return;
        
        OnTakeDamage?.Invoke(damageInfo.damage);
        
        
        if(healthBarTransform != null && damageInfo.damageType != DamageType.Gas||healthBarTransform != null && isBoss && damageInfo.damageType == DamageType.Gas)
        {
            CacularHealth(damageInfo);
            healthBarTransform.gameObject.SetActive(true);
            // Nếu đã có một Coroutine đang chờ ẩn thanh máu, hủy nó và tạo lại
            if (hideHealthBarCoroutine != null)
            {
                StopCoroutine(hideHealthBarCoroutine);
            }

            // Bắt đầu Coroutine để ẩn thanh máu sau 1 giây nếu không nhận thêm sát thương
            hideHealthBarCoroutine = StartCoroutine(HideHealthBarAfterDelay());
        }
        else if(damageInfo.damageType == DamageType.Gas)
        {
            // Nếu đã có một Coroutine đang chờ ẩn thanh máu, hủy nó và tạo lại
            if (!isBoss)
            {
                _currentHealth = 0;
                IsDeadExplosion = true;
                Die();
            }
            else
            {
                CacularHealth(damageInfo);
            }
            if (hideHealthBarCoroutine != null)
                StopCoroutine(hideHealthBarCoroutine);
                // Bắt đầu Coroutine để ẩn thanh máu sau 1 giây nếu không nhận thêm sát thương
            if(!IsDeadExplosion)
                hideHealthBarCoroutine = StartCoroutine(HideHealthBarAfterDelay());
        }
    }
    
    
    private void CheckImmortalStatus()
    {
        if (_currentHealth <= botConfigSO.health * (botConfigSO.HealthThreshold / 100f))
            isImmortal = true;
        else
            isImmortal = false;
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
        SpawnBotManager.Instance.RemoveBotDead(this);
        AchievementEvaluator.Instance.OnBotKilled(1f,false);
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

    public void SetFloatAnim(string _name, float value) => anim.SetFloat(_name, value);
    public void SetIntAnim(string _name, int value) => anim.SetInteger(_name, value);
    
    public void SetAnimAndType(string _name, int animType)
    {
        //print("SetAnimAndType: " + _name + " - " + animType + " - " + gameObject.name);
        anim.SetInteger("AnimType", animType);
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
    
    public void RotaToTarget()
    {
        Vector3 direction = LocalPlayer.Instance.GetLocalPlayer() - TF.position;
        Quaternion rotation = Quaternion.LookRotation(direction);

        Vector3 euler = rotation.eulerAngles;
        euler.x = 0f;
        TF.rotation = Quaternion.Euler(euler);
    }
    
    
    public int GetNearestDirection()
    {
        Vector3 toTarget = (_posDamageGas - TF.position).normalized;

        // Các hướng cơ bản trong local space
        Vector3 forward = TF.forward;
        Vector3 right = TF.right;
        Vector3 left = -TF.right;
        Vector3 back = -TF.forward;
        // 0 = trước
        // 1 = phải
        // 2 = trái
        // 3 = sau
        // Tính độ tương đồng (dot product)
        float dotForward = Vector3.Dot(toTarget, forward);
        float dotRight = Vector3.Dot(toTarget, right);
        float dotLeft = Vector3.Dot(toTarget, left);
        float dotBack = Vector3.Dot(toTarget, back);

        // Tìm hướng có dot lớn nhất (góc gần nhất)
        float maxDot = dotForward;
        int direction = 0; // 0 = trước

        if (dotRight > maxDot)
        {
            maxDot = dotRight;
            direction = 1;
        }

        if (dotLeft > maxDot)
        {
            maxDot = dotLeft;
            direction = 2;
        }

        if (dotBack > maxDot)
        {
            maxDot = dotBack;
            direction = 3;
        }

        return direction;
    }

    public void OnDespawn()
    {
        OnBotDead -= Die;
        //        print("Despawn BotNetwork: " + gameObject.name);
        if (isBotTutorial)
            gameObject.SetActive(false);
        else
            SimplePool.Despawn(this);
    }

    public void ExplosinArrmor()
    {
        if (canExplosionArmor)
        {
            foreach (BallisticArmor VARIABLE in arrBallisticArmor)
                if (VARIABLE.gameObject.activeSelf)
                    VARIABLE.gameObject.SetActive(false);
        }
    }
    
    public void PlayAudioVoice(int _index,float _volume)
    {
        _audioSourceVoice.volume = _volume;
        _audioSourceVoice.clip = _listSoundBotVoice[_index];
        _audioSourceVoice.Play();
    }
}


