using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ZombieBase : GameUnit, ITakeDamage
{
#if UNITY_EDITOR
    [SerializeField] private GameConstants.BotType botType;
    private void OnValidate()
    {
        PoolType = (GameConstants.PoolType)botType;
    }
#endif
    [Header("Data Bot")]
    [SerializeField] private BotConfigSO botConfigSO;
    [SerializeField] private Transform _centerBotZom;
    [SerializeField] protected bool isBotActiveEqualTay;
    public bool isBoss;
    
    [Header("Health Bar")]
    [SerializeField] private Transform mainCameraTranform;
    [SerializeField] protected Transform healthBarTransform;
    [SerializeField] private Image healthBarUI;
    [SerializeField] protected int _currentHealth;
    [SerializeField] protected bool isImmortal;
    [SerializeField] protected bool isDead;
    
    [Header("Bot Audio")]
    [Tooltip("Chạy những âm thanh phải dừng khi hết state")] [SerializeField] AudioSource _audioSourceVoice;
    [SerializeField] AudioClip[] _listSoundBotVoice;
    
    [Header("Point To Move")]
    [SerializeField] private WayPoint wayPoint;

    [Header("Change Anim")]
    [SerializeField] private string currentAnimName;
    [SerializeField] private int currentAnimType;
    [SerializeField] protected Animator animator;

    #region Get - Set
    public BotConfigSO BotConfigSO => botConfigSO;
    public bool IsDead => isDead;
    public int currentHealth => _currentHealth;
    public bool IsImmortal => isImmortal;
    public WayPoint GetWayPoint=> wayPoint;
    #endregion

    #region Actions
    public Action<int> OnTakeDamage { get; set; }
    public Action<bool> ZombieDead { get; set; }
    #endregion

    protected Coroutine hideHealthBarCoroutine; // Tham chiếu tới Coroutine
    private AudioManager _audioManager;

    #region Init - Despawn
    public virtual void OnInit()
    {
        _currentHealth = botConfigSO.health;
        isImmortal = false;
        isDead = false;
    }
    public virtual void OnInit(WayPoint _wayPoint)
    {
        wayPoint = _wayPoint;
        TF.position = _wayPoint.WayPoints[0].transform.position;
        _currentHealth = botConfigSO.health;
        isImmortal = false;
        isDead = false;
    }
    public void OnDespawn()
    {
        if(PoolType!=GameConstants.PoolType.None)
            SimplePool.Despawn(this);
        else
            gameObject.SetActive(false);
    }
    #endregion

    #region Base Unity

    protected virtual void Awake()
    {
        if (mainCameraTranform == null)
            mainCameraTranform = Camera.main.transform;
        
        if (healthBarTransform != null)
             healthBarTransform.gameObject.SetActive(false);
        
        
        if (isBotActiveEqualTay)
            OnInit();
    }

    private void Start()
    {
        _audioManager = AudioManager.Instance;
    }
    protected virtual void Update()
    {
        NUtiliti.AlignCamera(healthBarTransform, mainCameraTranform);
    }
    #endregion

    #region OnTakeDamage
    public virtual void TakeDamage(DamageInfo damageInfo)
    {
        
    }
    public void CacularHealth(DamageInfo damageInfo)
    {
        _currentHealth -= damageInfo.damage;
        SetHealthBar(_currentHealth);
        
        if (_currentHealth <= 0)
        {
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
        wayPoint.isUse = false;
        ZombieDead?.Invoke(true);
        isDead = true;
        _currentHealth = 0;
        if (healthBarTransform != null) 
            healthBarTransform.gameObject.SetActive(false);
        SpawnBotManager.Instance.RemoveBotDead(this);
    }
    #endregion
    
    #region Set Anim
    public void SetFloatAnim(string _name, float value) => animator.SetFloat(_name, value);
    public void SetIntAnim(string _name, int value) => animator.SetInteger(_name, value);
    public void ChangeAnim(string _name)
    {
#if UNITY_EDITOR
        if (animator == null)
        {
            Debug.LogError("Null anim");
            return;
        }
        // if (currentAnimName == _name)
        //     print("- "+_name+" |Old" + currentAnimName);
#endif
        animator.ResetTrigger(_name);
        currentAnimName = _name;
        animator.SetTrigger(currentAnimName);
    }
    
    public void ChangeAnimAndType(string _name, int animType)
    {
#if UNITY_EDITOR
        if (animator == null)
        {
            Debug.LogError("Null anim");
            return;
        }
        // if (currentAnimName == _name || currentAnimType == animType)
        //     Debug.LogWarning("Bạn đang lăp lại một anim: " + _name + " | Old: " + currentAnimName + " | Type: " + animType);
        // print("- " + _name + " |Old" + currentAnimName);
#endif
        animator.SetInteger("AnimType", animType);
        animator.ResetTrigger(_name);
        currentAnimName = _name; 
        currentAnimType = animType; 
        animator.SetTrigger(currentAnimName);
    }
    #endregion
    
    #region Audio Bot
    /// <summary>
    /// Play âm thanh của bot
    /// </summary>
    /// <param name="_index">index list clip</param>
    /// <param name="_volume">âm lượng</param>
    /// <param name="_audioSourceEqualThis">True: gọi StopAudioThis để dừng, False: không dừng được</param>
    public virtual void PlayAudioVoice(int _index,float _volume,bool _audioSourceEqualThis)
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

    public void PlayAudioVoiceLoop(int _index, float _volume)
    {
        _audioSourceVoice.clip = _listSoundBotVoice[_index];
        _audioSourceVoice.volume = _volume;
        _audioSourceVoice.Play();
    }

    public void StopAudioThis()
    {
        _audioSourceVoice.loop = false;
        _audioSourceVoice.Stop();
    }
    #endregion    
    
    public Transform GetTransformThis() => TF;
    public Transform GetTransformCenter() => _centerBotZom;

    public float DistanceToPlayermain() => Vector3.Distance(LocalPlayer.Instance.GetPosLocalPlayer(), TF.position);
    
    public void RotaToPlayerMain()
    {
        Vector3 direction = LocalPlayer.Instance.GetPosLocalPlayer() - TF.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        
        Vector3 euler = rotation.eulerAngles;
        euler.x = 0f;
        TF.rotation = Quaternion.Euler(euler);
    }
}
