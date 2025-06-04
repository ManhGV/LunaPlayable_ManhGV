using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameConstants;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class WeaponBase : Singleton<WeaponBase>
{
    [Header("Data Weapon")]
    [SerializeField] public WeaponInfo weaponInfo;

    [Header("Layer Target")]
    [SerializeField] private LayerMask botLayerMask;
    [SerializeField] private LayerMask armorBossLayerMask;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private LayerMask rewardLayerMask;
    [SerializeField] private LayerMask gasLayerMask;

    [Header("Snake Camera")]
    [SerializeField] private Transform shakeCam; // Biến để tham chiếu đến MainCamera
    [SerializeField] private float shakeCamMin;
    [SerializeField] private float shakeCamMax;
    [SerializeField] private bool IsShowLunaEndGame;

    protected Transform _cameraTransform;
    protected Camera _camera;
    protected float _timeSinceLastShoot = 0f; // Thời gian từ lần bắn cuối cùng
    protected int _currentBulletCount; // Số lượng đạn hiện tại trong băng
    protected int _defaultBulletCount; // Số lượng đạn hiện tại trong băng
    public int CurrentBulletCount => _currentBulletCount;
    public int DefaultBulletCount => _defaultBulletCount;
    protected bool _isReloading = false; // Trạng thái đang nạp đạn
    protected bool isShooting = false; // Trạng thái đang bắn
    protected bool canShoot = false; // Trạng thái có thể bắn
    protected Coroutine shootingCoroutine;

    [Header("DrawGizmod")]
    [SerializeField] LayerMask boxLayer;
    [SerializeField] private float distance;

    [Header("Lằng nhằng")]
    [SerializeField] protected Animation _animation;
    [SerializeField] protected PoolType _bulletType;  // Loại đạn sẽ được bắn
    [SerializeField] protected ParticleSystem[] _fireEffect;
    [SerializeField] protected AudioSource _audioSource;
    [SerializeField] protected AudioSource _audioSourceHit;
    [SerializeField] protected bool _isShowCard;

    [FormerlySerializedAs("_muzzleTrans1")]
    [FormerlySerializedAs("_muzzleTrans")]
    [Header("Muzzle")]
    [SerializeField] protected Transform _muzzleTrans_1;

    // kiểm tra có ang ấn vào UI không
    // Bộ nhớ tạm cho kiểm tra UI - static để tái sử dụng
    private static readonly PointerEventData PointerEventData = new PointerEventData(EventSystem.current);
    private static readonly List<RaycastResult> RaycastResults = new List<RaycastResult>();

    protected override void Awake()
    {
        base.Awake();
        _camera = Camera.main;
        _cameraTransform = _camera.transform;
        _currentBulletCount = weaponInfo.bulletCount; // Khởi tạo số lượng đạn
        _defaultBulletCount = _currentBulletCount;
        AssignAnimationClips();
        UIManager.Instance.GetUI<Canvas_GamePlay>().Init();
        Invoke(nameof(Init), .1f);
    }

    protected virtual void Start()
    {
        Instance = this;
    }

    protected virtual void Init()
    {
        EventManager.Invoke(EventName.UpdateBulletCount, _currentBulletCount);
        EventManager.Invoke(EventName.UpdateBulletCountDefault, _defaultBulletCount);
    }

    protected virtual void OnEnable()
    {
        EventManager.AddListener<bool>(EventName.OnShowLunaEndGame, OnShowLunaEndGame);
    }
    protected virtual void OnDisable()
    {
        EventManager.RemoveListener<bool>(EventName.OnShowLunaEndGame, OnShowLunaEndGame);
    }

    protected virtual void OnShowLunaEndGame(bool IsShow)
    {
        IsShowLunaEndGame = IsShow;
    }

    protected virtual void Update()
    {
#if UNITY_EDITOR
        GizmodTuVe();
#endif
        if (GameManager.Instance.GetGameState() != GameConstants.GameState.Playing)
            return;

        if (!IsPointerOverUI())
            OnShooting();
        if (false)//end game
        {
            isShooting = false;
            StopGunEffect();
        }
    }

    /// <summary>
    /// Kiểm tra xem con trỏ chuột có đang hover trên UI element không
    /// </summary>
    /// <returns>True nếu pointer đang trên UI, false nếu không</returns>
    public static bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;

        // Cập nhật vị trí con trỏ hiện tại
        PointerEventData.position = Input.mousePosition;
        RaycastResults.Clear();

        // Thực hiện raycast để kiểm tra UI elements
        EventSystem.current.RaycastAll(PointerEventData, RaycastResults);
        return RaycastResults.Count > 0;
    }

    protected virtual void AssignAnimationClips()
    {

    }

    private void OnShooting()
    {
        if (_isReloading)
            return;

        _timeSinceLastShoot += Time.deltaTime;

        if (Input.GetMouseButton(0))
        {
            LogicPlayGun();
        }
        else
        {
            LogicStopGun();
        }
    }

    private void LogicStopGun()
    {
        if (isShooting)
        {
            StopShootingSound();
            isShooting = false;
            canShoot = false; // Reset canShoot when stopping shooting
            if (shootingCoroutine != null)
            {
                StopCoroutine(shootingCoroutine);
                shootingCoroutine = null;
            }
            StopGunEffect(); // Dừng hiệu ứng nổ súng
        }
    }

    protected virtual void LogicPlayGun()
    {
        UICrosshairItem.Instance.Narrow_Crosshair();
        if (!isShooting)
        {
            isShooting = true;
            if (shootingCoroutine == null)
                shootingCoroutine = StartCoroutine(StartShootingAfterDelay());
        }

        if (canShoot && _timeSinceLastShoot >= weaponInfo.FireRate)
        {
            if (_currentBulletCount <= 0 && !weaponInfo.infiniteBullet)
                OnReload();
            else
            {
                Shoot();
                _timeSinceLastShoot = 0f;

                if (!weaponInfo.infiniteBullet)
                {
                    _currentBulletCount--;

                    EventManager.Invoke(EventName.UpdateBulletCount, _currentBulletCount);
                }
                PlayGunEffect(); // Kích hoạt hiệu ứng nổ súng
            }
        }
    }

    protected IEnumerator StartShootingAfterDelay()
    {
        yield return new WaitForSeconds(weaponInfo.WaitToShoot);
        canShoot = true;
        shootingCoroutine = null;
    }

    public Vector3 GizmodTuVe()
    {
        Ray ray = new Ray(shakeCam.position, shakeCam.forward);
        RaycastHit hit;

        Debug.DrawLine(ray.origin, ray.origin + ray.direction * distance, Color.red);

        // Bắn raycast
        if (Physics.Raycast(ray, out hit, distance, boxLayer))
        {
            // Debug.Log("Va chạm tại vị trí: " + hit.point);
            // Debug.Log("Khoảng cách đến box: " + hit.distance);

            Debug.DrawLine(hit.point, hit.point + Vector3.up * 0.1f, Color.green);
            return hit.point;
        }

        return Vector3.one;
    }

    protected virtual void Shoot()
    {
        if (this == null || _cameraTransform == null) return;

        Vector3 forward= _cameraTransform.forward;
            StartCoroutine(ShakeCamera(0.1f, 0.05f));

        //        print(_cameraTransform.forward + "Forward: " + forward);

        forward += new Vector3(
            Random.Range(-weaponInfo.recoilAmount, weaponInfo.recoilAmount),
            Random.Range(-weaponInfo.recoilAmount, weaponInfo.recoilAmount),
            Random.Range(-weaponInfo.recoilAmount, weaponInfo.recoilAmount)
        );

        // Bắn từ nòng đầu tiên
        FireFromMuzzle(_muzzleTrans_1, forward);

        _animation.Play("Fire");
        _animation["Fire"].speed = 2.0f;
        _audioSource.clip = weaponInfo.audioClip;
        _audioSource.Play();

        UICrosshairItem.Instance.Expand_Crosshair(15);

        PlayGunEffect();
    }

    #region Check Layer Target
    private bool IsInBotLayer(GameObject obj)
    {
        return ((1 << obj.layer) & botLayerMask) != 0;
    }

    private bool IsInArmorBossLayer(GameObject obj)
    {
        return ((1 << obj.layer) & armorBossLayerMask) != 0;
    }

    public static class LayerConstants
    {
        public static readonly int WeakPointLayer = 10; // Giả sử layer 10 là WeakPoint
        public static readonly LayerMask WeakPointMask = 1 << WeakPointLayer;
    }

    private bool IsInGroundLayerMask(GameObject obj)
    {
        return ((1 << obj.layer) & groundLayerMask) != 0;
    }

    private bool IsInRewardLayer(GameObject obj)
    {
        return ((1 << obj.layer) & rewardLayerMask) != 0;
    }

    private bool IsInGasLayer(GameObject obj)
    {
        return ((1 << obj.layer) & gasLayerMask) != 0;
    }
    #endregion

    protected virtual void FireFromMuzzle(Transform muzzle, Vector3 forward)
    {
        var shotRotation = Quaternion.Euler(Random.insideUnitCircle * weaponInfo.inaccuracy) * forward;
        var ray = new Ray(_cameraTransform.position, shotRotation);
        BulletTrail bullet = SimplePool.Spawn<BulletTrail>(_bulletType, muzzle.position, muzzle.rotation);
        Vector3 posGizmod = GizmodTuVe();
        bullet.Init((posGizmod - muzzle.position).normalized, posGizmod);

        bool CheckRayCast = Physics.Raycast(ray, out var hit, Mathf.Infinity, botLayerMask | armorBossLayerMask | gasLayerMask | rewardLayerMask | groundLayerMask );
        PoolType typeEffect = PoolType.vfx_ConcreteImpact;
        if (CheckRayCast)
        {
            //var damageType = hit.collider.CompareTag("WeakPoint")? DamageType.Weekness:DamageType.Normal;
            var damageType = hit.collider.gameObject.layer == LayerConstants.WeakPointLayer? DamageType.Weekness: DamageType.Normal;
            //Debug.Log($"Raycast hit object: {hit.collider.gameObject.name}, Layer: {hit.collider.gameObject.layer}");

            var damageInfo = new DamageInfo()
            {
                damageType = damageType,
                damage = weaponInfo.damage,
                name = hit.collider.name,
            };

            if (IsInBotLayer(hit.collider.gameObject))
            {
                var takeDamageController = hit.transform.gameObject.GetComponent<ITakeDamage>();
                if (takeDamageController == null)
                {
                    takeDamageController = hit.transform.root.gameObject.GetComponent<ITakeDamage>();
                }
                if (takeDamageController != null) takeDamageController.TakeDamage(damageInfo);
                typeEffect = PoolType.vfx_BloodEffectZom;
                // Debug.Log(damageType.ToString() + " " + weaponInfo.damage + " " + hit.collider.name);
            }
            else if (IsInArmorBossLayer(hit.collider.gameObject))
            {
                Detector detector = hit.transform.gameObject.GetComponent<Detector>();
                if (detector != null)
                    detector.SetHealthImage(damageInfo.damage);

                BallisticArmor ballisticArmor = hit.transform.gameObject.GetComponent<BallisticArmor>();
                if (ballisticArmor != null)
                    ballisticArmor.TakeDamage(damageInfo.damage);
                
                damageInfo.damage /= 3;
                var takeDamageController = hit.transform.gameObject.GetComponent<ITakeDamage>();
                if (takeDamageController == null)
                {
                    takeDamageController = hit.transform.root.gameObject.GetComponent<ITakeDamage>();
                }
                if (takeDamageController != null) takeDamageController.TakeDamage(damageInfo);
                typeEffect = PoolType.vfx_ShootGift;
                PlayRandomAttackSound();
            }
            else if (IsInRewardLayer(hit.collider.gameObject))
            {
                var rewardController = hit.transform.gameObject.GetComponent<IReward>();
                if (rewardController == null)
                {
                    rewardController = hit.transform.root.gameObject.GetComponent<IReward>();
                }
                if (rewardController != null) rewardController.TakeCollect(weaponInfo.damage);
                PlayRandomAttackSound();
                typeEffect = PoolType.vfx_ShootGift;
            }
            else if (IsInGroundLayerMask(hit.collider.gameObject))
            {
                var takeDamageController1 = hit.transform.gameObject.GetComponent<ITakeDamage>();
                if (takeDamageController1 == null)
                {
                    takeDamageController1 = hit.transform.root.gameObject.GetComponent<ITakeDamage>();
                }
                if (takeDamageController1 != null) takeDamageController1.TakeDamage(damageInfo);
                //Debug.Log(damageType.ToString() + " " + weaponInfo.damage + " " + hit.collider.name);
                typeEffect = PoolType.vfx_ConcreteImpact;
            }
            else if (IsInGasLayer(hit.collider.gameObject))
            {
                OxygenTanks oxygenTanks = null;
                oxygenTanks = hit.transform.gameObject.GetComponent<OxygenTanks>();
                if (oxygenTanks != null)
                    oxygenTanks.Explosion();
                typeEffect = PoolType.vfx_ConcreteImpact;
            }

            // Tạo hiệu ứng va chạm
            SimplePool.Spawn<Effect>(typeEffect, hit.point, Quaternion.identity).OnInit();
        }
        EventManager.Invoke(EventName.OnCheckBotTakeDamage, CheckRayCast);
    }

    void PlayRandomAttackSound()//kêu keng keng
    {
        // AudioClip clip = AudioManager.Instance.GetAudioAttackClip();
        // if (clip != null)    
        // {
        //     _audioSourceHit.PlayOneShot(clip);
        // }
    }

    #region Reload
    public void OnReload()
    {

        if (_isReloading || _currentBulletCount == _defaultBulletCount)
            return;
        _isReloading = true;
        StartCoroutine(Reload());
        StopGunEffect();
        UICrosshairItem.Instance.ResetCorosshair();
    }

    private IEnumerator Reload()
    {
        StopShootingSound();
        float reloadTime = weaponInfo.reloadTime;
        EventManager.Invoke(EventName.OnReloading, reloadTime);
        //Debug.Log("Reloading...");
        _audioSource.PlayOneShot(weaponInfo.AudioReloadIn);

        _animation.Play("ReloadIn");
        yield return new WaitForSeconds(reloadTime / 3);

        _animation.Play("ReloadOn");
        yield return new WaitForSeconds(reloadTime / 3);

        _audioSource.PlayOneShot(weaponInfo.AudioReloadOut);
        _animation.Play("ReloadOut");
        yield return new WaitForSeconds(reloadTime / 3);
        _currentBulletCount = weaponInfo.bulletCount;

        _isReloading = false;

        EventManager.Invoke(EventName.UpdateBulletCount, _currentBulletCount);
    }
    #endregion

    // Thêm phương thức dừng âm thanh bắn
    private void StopShootingSound()
    {
        if (_audioSource.isPlaying && _audioSource.clip == weaponInfo.audioClip)
            _audioSource.Stop();
    }

    // Thêm phương thức nhận AnimationEvent
    public void AnimationAudioEvent()
    {
        // Thực hiện hành động khi sự kiện AnimationAudioEvent được gọi
        //Debug.Log("AnimationAudioEvent called");
    }

    // Thêm hàm rung lắc camera
    protected IEnumerator ShakeCamera(float duration, float magnitude)
    {
        Quaternion originalRot = shakeCam.localRotation;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(shakeCamMin, shakeCamMax) * magnitude;
            float y = Random.Range(shakeCamMin, shakeCamMax) * magnitude;

            shakeCam.localRotation = originalRot * Quaternion.Euler(x, y, 0);

            elapsed += Time.deltaTime;

            yield return null;
        }

        shakeCam.localRotation = originalRot;
        EventManager.Invoke(EventName.OnCheckShakeCam, shakeCam.localEulerAngles);
    }

    #region Play - Stop Gun Effect
    protected virtual void PlayGunEffect()
    {
        foreach (ParticleSystem fireEffect in _fireEffect)
            if (fireEffect != null && !fireEffect.isPlaying)
                fireEffect.Play();
    }

    protected virtual void StopGunEffect()
    {
        foreach (ParticleSystem fireEffect in _fireEffect)
            if (fireEffect != null && fireEffect.isPlaying)
                fireEffect.Stop();
    }
    #endregion
}
