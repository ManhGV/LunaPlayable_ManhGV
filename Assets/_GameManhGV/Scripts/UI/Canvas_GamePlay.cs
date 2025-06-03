using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Canvas_GamePlay : UICanvas
{
    [Header("Game Process")] 
    [SerializeField] private Image _processFiller;
    [SerializeField] private Text _processText;
    
    [Header("Buttons Reload")]
    [SerializeField] GameObject btnReload;
    [SerializeField] private GameObject AmmoQuantity;
    [SerializeField] Text bulletCountDefault;
    [SerializeField] Text bulletCountCurrent;
    
    [Header("WeaponCircleReloading")]
    [Tooltip("Icon Weapon ở góc dưới trái")] [SerializeField] private Image iconWeapon;
    [Tooltip("Icon Reload ở góc dưới trái")] [SerializeField] private Image iconreload;
    [Space(20)]
    [SerializeField] private GameObject WeaponCircleReloading;
    [Tooltip("Icon Reload ở giữa")] [SerializeField] private Image reloadIcon;
    [Tooltip("Text Reload ở giữa")] [SerializeField] private Text reloadTime;
    [SerializeField] private float rotationSpeed = 250f;

    [Header("Crosshair")] 
    [SerializeField] private GameObject crosshair;
    
    [Header("Buttons Rocket")]
    [SerializeField] GameObject btnRocket;

    [Header("Powerup Weapon")]
    [SerializeField] private GameObject _powerupEffectUI;
    
    private bool instructReload = false;
    private bool instructRocket = false;
    private Weapon26 weapon26;
    public void Init()
    {
        EventManager.AddListener<float>(EventName.UpdateGameProcess, UpdateGameProcess);
        EventManager.AddListener<int>(EventName.UpdateBulletCount, UpdateBulletCount);
        EventManager.AddListener<int>(EventName.UpdateBulletCountDefault, UpdateBulletCountDefault);
        EventManager.AddListener<float>(EventName.OnReloading, OnReloading);
        
        EventManager.AddListener<bool>(EventName.InstructReload, InstructReload);
        EventManager.AddListener<bool>(EventName.InstructRocket, InstructRocket );
        weapon26 = (Weapon26)WeaponBase.Instance;
    }

    private void UpdateGameProcess(float arg0)
    {
        _processFiller.fillAmount = arg0;
        SpawnBotManager spawnBotManager = SpawnBotManager.Instance;
        _processText.text = spawnBotManager.KillBot+" / " + spawnBotManager.AllBotsToSpawn;
    }

    private void OnReloading(float _timeReload)
    {
        StartCoroutine(IERotation(_timeReload));
    }

    IEnumerator IERotation(float _timeReload)
    {
        iconreload.gameObject.SetActive(true);
        iconWeapon.gameObject.SetActive(false);
        WeaponCircleReloading.SetActive(true);
        AmmoQuantity.SetActive(false);
        crosshair.SetActive(false);
        while (true)
        {
            _timeReload -= Time.deltaTime;
            reloadTime.text = _timeReload.ToString("F1");
            if (_timeReload >= 0)
            {
                iconreload.transform.Rotate(Vector3.forward, -rotationSpeed * Time.deltaTime);
                reloadIcon.transform.Rotate(Vector3.forward, -rotationSpeed * Time.deltaTime);
            }
            else
            { 
                crosshair.SetActive(true);
                AmmoQuantity.SetActive(true);
                iconreload.gameObject.SetActive(false);
                iconWeapon.gameObject.SetActive(true);
                WeaponCircleReloading.SetActive(false);
                break;
            }
            yield return null;
        }
    }

    private void UpdateBulletCountDefault(int _bulletCount)
    {
        bulletCountCurrent.text = _bulletCount.ToString();
    }

    private void UpdateBulletCount(int _bulletCount)
    {
        bulletCountDefault.text = _bulletCount.ToString();
    }

    public override void Setup()
    {
        btnReload.gameObject.SetActive(false);
        btnRocket.gameObject.SetActive(false);
        base.Setup();
    }
    
    #region Instruct

    void InstructReload(bool _isActiveReloadBtn)
    {
        btnReload.gameObject.SetActive(true);
        GameManager.Instance.PauseGame();
        UIManager.Instance.OpenUI<Canvas_ReloadIntroduction>();
    }

    private void InstructRocket(bool arg0)
    {
        btnRocket.SetActive(true);
        GameManager.Instance.PauseGame();
        UIManager.Instance.OpenUI<Canvas_BazookaIntroduction>();
    }

    #endregion
    
    public void Btn_Reload()
    {
        if (!instructReload)
        {
            instructReload = true;
            GameManager.Instance.ResumeGame();
            UIManager.Instance.CloseUIDirectly<Canvas_ReloadIntroduction>();
        }
        weapon26.OnReload();
    }
    
    public void Btn_Rocket()
    {
        if (!instructRocket)
        {
            instructRocket = true;
            GameManager.Instance.ResumeGame();
            UIManager.Instance.CloseUIDirectly<Canvas_BazookaIntroduction>();
        }
        RocketController.Instance.Fire();
    }

    public void PowerupEffectUI()
    {
        _powerupEffectUI.SetActive(true);
    }
}