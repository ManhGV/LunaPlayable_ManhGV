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
    [SerializeField] private GameObject AmmoQuantity;
    [SerializeField] Text bulletCountDefault;
    [SerializeField] Text bulletCountCurrent;
    
    [Header("WeaponCircleReloading")]
    [Tooltip("Icon Weapon ở góc dưới trái")] [SerializeField] private Image iconWeapon;
    [Tooltip("Icon Reload ở góc dưới trái")] [SerializeField] private Image iconreload;
    Coroutine _reloadCoroutine;
    
    [Space(20)]
    [SerializeField] private GameObject WeaponCircleReloading;
    [Tooltip("Icon Reload ở giữa")] [SerializeField] private Image reloadIcon;
    [Tooltip("Text Reload ở giữa")] [SerializeField] private Text reloadTime;
    [SerializeField] private float rotationSpeed = 250f;

    [Header("Crosshair")] 
    [SerializeField] private GameObject crosshair;
    
    [Header("Buttons Rocket")]
    [SerializeField] GameObject getMoreRocket;
    [SerializeField] Image rocketFill;
    [SerializeField] Text rocketCooldown;
    [SerializeField] Text rocketAmount;
    Coroutine caculateFillCoroutine;

    [Header("Powerup Weapon")]
    [SerializeField] private GameObject _powerupEffectUI;

    [Header("Active Reload")] 
    [SerializeField] CanvasGroup _canvasGrupReloadFast;
    [SerializeField] CanvasGroup _canvasGrupReloadFastFake;
    [SerializeField] private Animator _animatorReloadFast;
    [SerializeField] Button _tapToReload;
    [SerializeField] Text _timeToReloadFast;
    [SerializeField] RectTransform uiElementPointer;  // Kéo UI element từ canvas vào đây trong inspector
    [SerializeField] RectTransform  startPos;
    [SerializeField] RectTransform  endPos;
    
    [Header("Open End Game")]
    [SerializeField] CanvasGroup _canvasGroupThis;
    [SerializeField] CanvasGroup _canvasGroupEndGame;
    [SerializeField] GameObject _endGameWonPanel;
    [SerializeField] GameObject _endGameLosePanel;
    
    public void Init()
    {
        EventManager.AddListener<float>(EventName.UpdateGameProcess, UpdateGameProcess);
        EventManager.AddListener<int>(EventName.UpdateBulletCount, UpdateBulletCount);
        EventManager.AddListener<int>(EventName.UpdateBulletCountDefault, UpdateBulletCountDefault);
        EventManager.AddListener<float>(EventName.OnReloading, OnReloading);
    }

    private void UpdateGameProcess(float arg0)
    {
        _processFiller.fillAmount = arg0;
        SpawnBotManager spawnBotManager = SpawnBotManager.Instance;
        _processText.text = spawnBotManager.KillBot+" / " + spawnBotManager.AllBotsToSpawn;
    }

    private void OnReloading(float _timeReload)
    {
        if(_reloadCoroutine != null)
            StopCoroutine(_reloadCoroutine);
        
        _reloadCoroutine = StartCoroutine(IERotation(_timeReload));
    }
    
    IEnumerator IERotation(float _timeReload)
    {
        _canvasGrupReloadFastFake.alpha = 1;
        iconreload.gameObject.SetActive(true);
        iconWeapon.gameObject.SetActive(false);
        WeaponCircleReloading.SetActive(true);
        AmmoQuantity.SetActive(false);
        crosshair.SetActive(false);
        
        uiElementPointer.anchoredPosition = startPos.anchoredPosition;//
        float elapsed = 0;//
        float duration = _timeReload;//
        
        while (true)
        {
            _timeReload -= Time.deltaTime;
            elapsed += Time.deltaTime;//
            _timeToReloadFast.text = _timeReload.ToString("F1");
            reloadTime.text = _timeReload.ToString("F1");
            if (_timeReload >= 0)
            {
                iconreload.transform.Rotate(Vector3.forward, -rotationSpeed * Time.deltaTime);
                reloadIcon.transform.Rotate(Vector3.forward, -rotationSpeed * Time.deltaTime);
                
                float t = Mathf.Clamp01(elapsed / duration); // Từ 0 → 1
                uiElementPointer.anchoredPosition = Vector2.Lerp(startPos.anchoredPosition, endPos.anchoredPosition, t);//
            }
            else
            {
                _canvasGrupReloadFast.alpha = 0;
                _canvasGrupReloadFastFake.alpha = 0;
                _tapToReload.gameObject.SetActive(false);
                
                crosshair.SetActive(true);
                AmmoQuantity.SetActive(true);
                iconreload.gameObject.SetActive(false);
                iconWeapon.gameObject.SetActive(true);
                WeaponCircleReloading.SetActive(false);
                break;
            }
            yield return null;
        }
        
        uiElementPointer.anchoredPosition = endPos.anchoredPosition;
        _reloadCoroutine = null;
    }

    private void UpdateBulletCountDefault(int _bulletCount)
    {
        bulletCountCurrent.text = _bulletCount.ToString();
    }

    private void UpdateBulletCount(int _bulletCount)
    {
        bulletCountDefault.text = _bulletCount.ToString();
    }
    
    public void Btn_Reload()
    {
        ((ReloadableWeapons)WeaponBase.Instance).OnReload();
    }
    
    public void Btn_Rocket()
    {
        if(caculateFillCoroutine != null)
            return;
        RocketController.Instance.Fire();
        caculateFillCoroutine = StartCoroutine(CaculatorFillCooldown());
    }

    public void ActiveReloadFast()
    {
        //GameManager.Instance.SlomotionTimeScale();
        //_activeReloadFast.gameObject.SetActive(true);
        _tapToReload.gameObject.SetActive(true);
    }
    
    public void Btn_ReloadFast()
    {
        StartCoroutine(IEAnimActiveReloadFast());
        
        
        if(_reloadCoroutine != null)
            StopCoroutine(_reloadCoroutine);
        
        _tapToReload.gameObject.SetActive(false);
        
        crosshair.SetActive(true);
        AmmoQuantity.SetActive(true);
        iconreload.gameObject.SetActive(false);
        iconWeapon.gameObject.SetActive(true);
        WeaponCircleReloading.SetActive(false);
    }
    
    private IEnumerator IEAnimActiveReloadFast()
    {
        _canvasGrupReloadFastFake.alpha = 0;
        _canvasGrupReloadFast.alpha = 1;
        if(Vector3.Distance(uiElementPointer.anchoredPosition, endPos.anchoredPosition) > 272.5f)//perfect reload
        {
            ((ReloadableWeapons)WeaponBase.Instance).OnReloadFast(80);
            _animatorReloadFast.Play("Perfect",0,0f);
            _timeToReloadFast.text = "0.0";
        }
        else
        {
            ((ReloadableWeapons)WeaponBase.Instance).OnReloadFast(40);
            _animatorReloadFast.Play("Good",0,0f);
            _timeToReloadFast.text = "0.5";
        }
        yield return new WaitForSeconds(0.5f);
        //_activeReloadFast.gameObject.SetActive(false);
    }

    public IEnumerator CaculatorFillCooldown()
    {
        RocketController rocketController = RocketController.Instance;
        rocketAmount.text = rocketController.currentAmount.ToString();
        if (rocketController.currentAmount <= 0)
        {
            getMoreRocket.SetActive(true);
        }
        else
        {
            rocketFill.gameObject.SetActive(true);
            while (rocketController.timer >= 0)
            {
                rocketFill.fillAmount = rocketController.GetFillAmount();
                rocketCooldown.text = rocketController.timer.ToString("F1");
                yield return null;
            }

            rocketFill.fillAmount = 0;
            caculateFillCoroutine = null;
            rocketFill.gameObject.SetActive(false);
        }
    }

    public void PowerupEffectUI()
    {
        _powerupEffectUI.SetActive(true);
    }

    public void OpendEndGame(bool _isWin)
    {
        if(_isWin)
            StartCoroutine(IEOpenEndGameWin());
        else
            StartCoroutine(IEOpenEndGameLose());
    } 
    
    public IEnumerator IEOpenEndGameWin()
    {
        WeaponBase.Instance.StopGunEffect();
        _canvasGroupThis.alpha = 1f;
        yield return new WaitForSeconds(2f);
        this.RunOnSeconds(1f, () => _canvasGroupThis.alpha -= Time.deltaTime);
        yield return new WaitForSeconds(.2f);
        _endGameWonPanel.SetActive(true);
        _canvasGroupEndGame.alpha = 0f;
        this.RunOnSeconds(1f, () => _canvasGroupEndGame.alpha += Time.deltaTime);
        yield return new WaitForSeconds(.9f);
        _endGameWonPanel.SetActive(true);
    }
    
    public IEnumerator IEOpenEndGameLose()
    {
        WeaponBase.Instance.StopGunEffect();
        _canvasGroupThis.alpha = 1f;
        yield return new WaitForSeconds(1.4f);
        this.RunOnSeconds(1f, () => _canvasGroupThis.alpha -= Time.deltaTime);
        yield return new WaitForSeconds(.2f);
        _endGameLosePanel.SetActive(true);
        _canvasGroupEndGame.alpha = 0f;
        this.RunOnSeconds(1f, () => _canvasGroupEndGame.alpha += Time.deltaTime);
        yield return new WaitForSeconds(.9f);
        _endGameLosePanel.SetActive(true);
    }
    public void UpdateBulletChangeWeapon()
    {
        UpdateBulletCount(WeaponBase.Instance.weaponInfo.bulletCount);
        UpdateBulletCountDefault(WeaponBase.Instance.weaponInfo.bulletCount);
    }
}