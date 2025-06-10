using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZombieBase : GameUnit, ITakeDamage
{
#if UNITY_EDITOR
    [SerializeField] private GameConstants.BotType botType;
#endif
    [Header("Data Bot")]
    [SerializeField] private BotConfigSO botConfigSO;
    [SerializeField] protected bool isBotActiveEqualTay;
    public bool isBoss;
    
    [Header("Health Bar")]
    [SerializeField] private Transform mainCameraTranform;
    [SerializeField] protected Transform healthBarTransform;
    [SerializeField] private Image healthBarUI;
    [SerializeField] protected int _currentHealth;
    [SerializeField] private bool isImmortal;
    [SerializeField] protected bool isDead;
    
    [Header("Bot Audio")]
    [Tooltip("Chạy những âm thanh phải dừng khi hết state")] [SerializeField] AudioSource _audioSourceVoice;
    [SerializeField] AudioClip[] _listSoundBotVoice;
    
    [Header("Point To Move")]
    [SerializeField] private WayPoint wayPoint;

    [Header("Change Anim")]
    [SerializeField] private string currentAnimName;
    [SerializeField] private Animator anim;

    #region Get - Set
    public BotConfigSO BotConfigSO => botConfigSO;
    public bool IsDead => isDead;
    public int currentHealth => _currentHealth;
    public bool IsImmortal => isImmortal;
    public WayPoint GetWayPoint=> wayPoint;
    #endregion

    #region Actions
    public Action<int> OnTakeDamage { get; set; }
    public Action<float> OnHealthChanged { get; set; }
    public Action OnBotDead { get; set; }
    #endregion

    protected Coroutine hideHealthBarCoroutine; // Tham chiếu tới Coroutine
    private AudioManager _audioManager;

    #region Init - Despawn
    public virtual void OnInit()
    {
        _currentHealth = botConfigSO.health;
        isImmortal = false;
        isDead = false;
        OnBotDead += BotDead;
    }
    public virtual void OnInit(WayPoint _wayPoint)
    {
        wayPoint = _wayPoint;
        TF.position = _wayPoint.WayPoints[0].transform.position;
        _currentHealth = botConfigSO.health;
        isImmortal = false;
        isDead = false;
        OnBotDead += BotDead;
    }
    public void OnDespawn()
    {
        OnBotDead -= BotDead;
        SimplePool.Despawn(this);
    }
    #endregion

    #region Base Unity

    private void OnValidate()
    {
        PoolType = (GameConstants.PoolType)botType;
    }

    protected virtual void Awake()
    {
        if (mainCameraTranform == null)
            mainCameraTranform = Camera.main.transform;
        
        if (healthBarTransform != null)
             healthBarTransform.gameObject.SetActive(false);
    }

    private void Start()
    {
        _audioManager = AudioManager.Instance;
    }
    private void Update()
    {
        NUtiliti.AlignCamera(healthBarTransform, mainCameraTranform);
    }
    #endregion

    #region OnTakeDamage
    public virtual void TakeDamage(DamageInfo damageInfo)
    {   
        if(isDead) 
            return;
        OnTakeDamage?.Invoke(damageInfo.damage);
    }
    public void CacularHealth(DamageInfo damageInfo)
    {
        _currentHealth -= damageInfo.damage;
        SetHealthBar(_currentHealth);
        
        if (_currentHealth <= 0)
        {
            isDead = true;
            BotDead();
        }
    }
    private void SetHealthBar(float currentHealth)
    {
        float healthBarValue = (currentHealth / botConfigSO.health);
        if (healthBarUI != null)
            healthBarUI.fillAmount = healthBarValue;
    }
    
    protected IEnumerator IEHideHealthBarAfterDelay()
    {
        if (isDead)
            healthBarTransform.gameObject.SetActive(false);
        yield return new WaitForSeconds(2f);
        // Ẩn thanh máu nếu bot chưa chết
        if (!isDead)
            healthBarTransform.gameObject.SetActive(false);
        
        hideHealthBarCoroutine = null;
    }
    
    public virtual void BotDead()
    {
        isDead = true;
        _currentHealth = 0;
        if (healthBarTransform != null) 
            healthBarTransform.gameObject.SetActive(false);
    }
    #endregion
    
    #region Set Anim
    public void SetFloatAnim(string _name, float value) => anim.SetFloat(_name, value);
    public void SetIntAnim(string _name, int value) => anim.SetInteger(_name, value);
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
    #endregion
    
    #region Audio Bot
    /// <summary>
    /// Play âm thanh của bot
    /// </summary>
    /// <param name="_index">index list clip</param>
    /// <param name="_volume">âm lượng</param>
    /// <param name="_audioSourceEqualThis">True: gọi StopAudioThis để dừng, False: không dừng được</param>
    public void PlayAudioVoice(int _index,float _volume,bool _audioSourceEqualThis)
    {
        if (_audioSourceEqualThis)
        {
            _audioSourceVoice.clip = _listSoundBotVoice[_index];
            _audioSourceVoice.volume = _volume;
            _audioSourceVoice.Play();
        }
        else
           _audioManager.PlaySound(_listSoundBotVoice[_index],_volume);
    }

    public void StopAudioThis() => _audioSourceVoice.Stop();
    #endregion    
    
    public Transform GetTransform() => TF;
                  
    public void RotaToTarget()
    {
        Vector3 direction = LocalPlayer.Instance.GetLocalPlayer() - TF.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        
        Vector3 euler = rotation.eulerAngles;
        euler.x = 0f;
        TF.rotation = Quaternion.Euler(euler);
    }
}
