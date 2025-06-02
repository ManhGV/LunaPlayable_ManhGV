using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameConstants;
using UnityEngine.EventSystems;

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

    [Header("Muzzle")]
    [SerializeField] private Transform _muzzleTrans;

    [SerializeField] private Animation _animation;
    [SerializeField] private PoolType _bulletType; // Lo?i ??n s? ???c b?n
    [SerializeField] private ParticleSystem[] _fireEffect;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioSource _audioSourceHit;
    [SerializeField] private bool _isShowCard;

    [Header("Snake Camera")]
    [SerializeField] private Transform shakeCam; // Bi?n ?? tham chi?u ??n MainCamera
    [SerializeField] private float shakeCamMin;
    [SerializeField] private float shakeCamMax;
    [SerializeField] private bool IsShowLunaEndGame;

    private Transform _cameraTransform;
    private Camera _camera;
    private float _timeSinceLastShoot = 0f; // Th?i gian t? l?n b?n cu?i cùng
    private int _currentBulletCount; // S? l??ng ??n hi?n t?i trong b?ng
    private int _defaultBulletCount; // S? l??ng ??n hi?n t?i trong b?ng
    public int CurrentBulletCount => _currentBulletCount;
    public int DefaultBulletCount => _defaultBulletCount;
    private bool _isReloading = false; // Tr?ng thái ?ang n?p ??n
    private float currentRotationSpeed = 0f; // T?c ?? quay hi?n t?i c?a nòng súng
    private bool isShooting = false; // Tr?ng thái ?ang b?n
    private bool canShoot = false; // Tr?ng thái có th? b?n
    private bool isBarrelSpinning = false; // Tr?ng thái nòng súng ?ang quay
    private Coroutine shootingCoroutine;

    [Header("DrawGizmod")]

    [SerializeField] private float distance;

    [Header("Tutorial")]
    public bool instructReload = false;
    public bool instructRoket = false;

    // ki?m tra có ang ?n vào UI không
    // B? nh? t?m cho ki?m tra UI - static ?? tái s? d?ng
    private static readonly PointerEventData PointerEventData = new PointerEventData(EventSystem.current);
    private static readonly List<RaycastResult> RaycastResults = new List<RaycastResult>();

    protected override void Awake()
    {
        base.Awake();
        _bulletType = PoolType.Projectile_Bullet_Norman;
        _camera = Camera.main;
        _cameraTransform = _camera.transform;
        _currentBulletCount = weaponInfo.bulletCount; // Kh?i t?o s? l??ng ??n
        _defaultBulletCount = _currentBulletCount;
        AssignAnimationClips();
        UIManager.Instance.GetUI<Canvas_GamePlay>().Init();
        Invoke(nameof(Init), .1f);
    }

    private void Start()
    {
        Instance = this;
    }

    private void Init()
    {
        EventManager.Invoke(EventName.UpdateBulletCount, _currentBulletCount);
        EventManager.Invoke(EventName.UpdateBulletCountDefault, _defaultBulletCount);
    }

    private void OnEnable()
    {
        EventManager.AddListener<bool>(EventName.OnShowLunaEndGame, OnShowLunaEndGame);
        EventManager.AddListener<bool>(EventName.OnChangeFireRate, OnChangeFireRate);
    }
    private void OnDisable()
    {
        EventManager.RemoveListener<bool>(EventName.OnShowLunaEndGame, OnShowLunaEndGame);
        EventManager.RemoveListener<bool>(EventName.OnChangeFireRate, OnChangeFireRate);
    }

    private void OnShowLunaEndGame(bool IsShow)
    {
        IsShowLunaEndGame = IsShow;
    }

    public bool IsReloadFull()
    {
        return _currentBulletCount >= weaponInfo.bulletCount;
    }

    private void Update()
    {
#if UNITY_EDITOR
        GizmodTuVe();
#endif
        if (GameManager.Instance.GetGameState() != GameConstants.GameState.Playing)
            return;

        HandleGatlingGunRotation();
        // if (!UIEndGame.Instance.IsShowEndGame)
        // {
        if (!IsPointerOverUI())
            OnShooting();
        // }
        // if (UIEndGame.Instance.IsShowEndGame )
        // {
        //     isShooting = false;
        //     StopGunEffect();
        // }


        if (Input.GetKeyDown(KeyCode.R))
        {
            InstructRocket();
        }

    }

    public void InstructRocket()
    {
        //print("H??ng d?n rocket");
        if (!instructRoket)
        {
            instructRoket = true;
            EventManager.Invoke(EventName.InstructRocket, true);
        }
    }

    public void InstructReload()
    {
        //TODO: n?u ch?a h??ng d?n reload thì h??ng d?n reload
        if (!instructReload)
        {
            instructReload = true;
            EventManager.Invoke(EventName.InstructReload, true);
        }
    }

    /// <summary>
    /// Ki?m tra xem con tr? chu?t có ?ang hover trên UI element không
    /// </summary>
    /// <returns>True n?u pointer ?ang trên UI, false n?u không</returns>
    public static bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;

        // C?p nh?t v? trí con tr? hi?n t?i
        PointerEventData.position = Input.mousePosition;
        RaycastResults.Clear();

        // Th?c hi?n raycast ?? ki?m tra UI elements
        EventSystem.current.RaycastAll(PointerEventData, RaycastResults);
        return RaycastResults.Count > 0;
    }

    private void AssignAnimationClips()
    {
        if (_animation != null && weaponInfo != null)
        {
            _animation.AddClip(weaponInfo.Fire, "Fire");
            _animation.AddClip(weaponInfo.Idle, "Idle");
            _animation.AddClip(weaponInfo._reloadAnimIn, "ReloadIn");
            _animation.AddClip(weaponInfo._reloadAnimOn, "ReloadOn");
            _animation.AddClip(weaponInfo._reloadAnimOut, "ReloadOut");
        }
    }

    private void OnShooting()
    {
        if (_isReloading)//IsIngameGUI || 
            return;

        _timeSinceLastShoot += Time.deltaTime;

        if (Input.GetMouseButton(0))
        {
            UICrosshairItem.Instance.Narrow_Crosshair();
            if (!isShooting)
            {
                isShooting = true;
                if (shootingCoroutine == null)
                {
                    shootingCoroutine = StartCoroutine(StartShootingAfterDelay());
                }
                if (!isBarrelSpinning)//&& !EventSystem.current.IsPointerOverGameObject()
                {
                    _audioSource.clip = weaponInfo.AudioStartBarrel;
                    _audioSource.Play();
                    isBarrelSpinning = true;
                }
            }

            if (canShoot && _timeSinceLastShoot >= weaponInfo.FireRate)
            {
                if (_currentBulletCount <= 0 && !weaponInfo.infiniteBullet)
                {
                    OnReload();
                }
                else
                {
                    Shoot();
                    _timeSinceLastShoot = 0f;

                    if (!weaponInfo.infiniteBullet)
                    {
                        _currentBulletCount--;
                        if (_currentBulletCount <= 3 && !instructReload)
                        {
                            instructReload = true;
                            EventManager.Invoke(EventName.InstructReload, true);
                        }

                        //Debug.Log("Bullet fired. Remaining bullets: " + _currentBulletCount);
                        EventManager.Invoke(EventName.UpdateBulletCount, _currentBulletCount);
                    }
                    PlayGunEffect(); // Kích ho?t hi?u ?ng n? súng
                }
            }
        }
        else
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
                if (isBarrelSpinning)
                {
                    _audioSource.clip = weaponInfo.AudioEndBarrel;
                    _audioSource.Play();
                    isBarrelSpinning = false;
                }
                StopGunEffect(); // D?ng hi?u ?ng n? súng
            }
        }
    }
    public void OnReload()
    {

        if (_isReloading || _currentBulletCount == _defaultBulletCount)
            return;
        _isReloading = true;
        StartCoroutine(Reload());
        StopGunEffect();
        UICrosshairItem.Instance.ResetCorosshair();
    }
    public int GetCurrentAmmo()
    {
        return _currentBulletCount;
    }
    private void HandleGatlingGunRotation()
    {
        if (weaponInfo.isGatlingGun)
        {
            if (isShooting)
            {
                currentRotationSpeed += (weaponInfo.MaxSpeedRotaBarrel / Mathf.Max(1, weaponInfo.WaitToShoot)) * Time.deltaTime;
                if (currentRotationSpeed >= weaponInfo.MaxSpeedRotaBarrel)
                {
                    currentRotationSpeed = weaponInfo.MaxSpeedRotaBarrel;
                }
            }
            else if (currentRotationSpeed > weaponInfo.MinSpeedRotaBarrel)
            {
                currentRotationSpeed -= (weaponInfo.MaxSpeedRotaBarrel / weaponInfo.TimeMinSpeed) * Time.deltaTime;
                if (currentRotationSpeed <= weaponInfo.MinSpeedRotaBarrel)
                {
                    currentRotationSpeed = weaponInfo.MinSpeedRotaBarrel;
                }
            }
        }
    }

    private IEnumerator StartShootingAfterDelay()
    {
        yield return new WaitForSeconds(weaponInfo.WaitToShoot);
        canShoot = true;
        shootingCoroutine = null;
    }


    [SerializeField] LayerMask boxLayer;
    public Vector3 GizmodTuVe()
    {
        Ray ray = new Ray(shakeCam.position, shakeCam.forward);
        RaycastHit hit;

        Debug.DrawLine(ray.origin, ray.origin + ray.direction * distance, Color.red);

        // B?n raycast
        if (Physics.Raycast(ray, out hit, distance, boxLayer))
        {
            // Debug.Log("Va ch?m t?i v? trí: " + hit.point);
            // Debug.Log("Kho?ng cách ??n box: " + hit.distance);

            Debug.DrawLine(hit.point, hit.point + Vector3.up * 0.1f, Color.green);
            return hit.point;
        }

        return Vector3.one;
    }

    private void Shoot()
    {
        if (this == null || _cameraTransform == null) return;

        Vector3 forward=_cameraTransform.forward;
        StartCoroutine(ShakeCamera(0.1f, 0.05f));

        forward += new Vector3(
            Random.Range(-weaponInfo.recoilAmount, weaponInfo.recoilAmount),
            Random.Range(-weaponInfo.recoilAmount, weaponInfo.recoilAmount),
            Random.Range(-weaponInfo.recoilAmount, weaponInfo.recoilAmount)
        );

        // B?n t? nòng ??u tiên
        FireFromMuzzle(_muzzleTrans, forward);

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
        public static readonly int WeakPointLayer = 10; // Gi? s? layer 10 là WeakPoint
        public static readonly LayerMask WeakPointMask = 1 << WeakPointLayer;
    }

    private bool IsInGroundLayerMask(GameObject obj)
    {
        return ((1 << obj.layer) & (groundLayerMask | LayerConstants.WeakPointMask)) != 0;
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


    private void FireFromMuzzle(Transform muzzle, Vector3 forward)
    {
        var shotRotation = Quaternion.Euler(Random.insideUnitCircle * weaponInfo.inaccuracy) * forward;
        var ray = new Ray(_cameraTransform.position, shotRotation);

        BulletTrail bullet = SimplePool.Spawn<BulletTrail>(_bulletType, muzzle.position, muzzle.rotation);
        Vector3 posGizmod = GizmodTuVe();
        bullet.Init((posGizmod - muzzle.position).normalized, posGizmod);

        bool CheckRayCast = Physics.Raycast(ray, out var hit, Mathf.Infinity, botLayerMask | armorBossLayerMask | gasLayerMask | rewardLayerMask | groundLayerMask | LayerConstants.WeakPointMask);
        PoolType typeEffect = PoolType.vfx_ConcreteImpact;
        if (CheckRayCast)
        {
            //var damageType = hit.collider.CompareTag("WeakPoint")? DamageType.Weekness:DamageType.Normal;
            var damageType = hit.collider.gameObject.layer == LayerConstants.WeakPointLayer
                ? DamageType.Weekness
                : DamageType.Normal;
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
                //                Debug.Log(damageType.ToString() + " " + weaponInfo.damage + " " + hit.collider.name);
            }
            else if (IsInArmorBossLayer(hit.collider.gameObject))
            {
                Detector detector = hit.transform.gameObject.GetComponent<Detector>();
                if (detector != null)
                    detector.SetHealthImage(damageInfo.damage);

                damageInfo.damage /= 3;
                var takeDamageController = hit.transform.gameObject.GetComponent<ITakeDamage>();
                if (takeDamageController == null)
                {
                    takeDamageController = hit.transform.root.gameObject.GetComponent<ITakeDamage>();
                }
                if (takeDamageController != null) takeDamageController.TakeDamage(damageInfo);

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
                typeEffect = PoolType.vfx_ConcreteImpact;
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
                //PlayRandomAttackSound();
                typeEffect = PoolType.vfx_ConcreteImpact;
            }
            else if (IsInGasLayer(hit.collider.gameObject))
            {
                // BinhGa binhGa = null;
                // binhGa = hit.transform.gameObject.GetComponent<BinhGa>();
                // if(binhGa !=null)
                //     binhGa.Explosion();
                //PlayRandomAttackSound();
                typeEffect = PoolType.vfx_ConcreteImpact;
                // print("Ban vao Ga");
            }

            // T?o hi?u ?ng va ch?m
            SimplePool.Spawn<Effect>(typeEffect, hit.point, Quaternion.identity).Init();
        }
        EventManager.Invoke(EventName.OnCheckBotTakeDamage, CheckRayCast);
    }


    void PlayRandomAttackSound()
    {
        // AudioClip clip = AudioManager.Instance.GetAudioAttackClip();
        // if (clip != null)    
        // {
        //     //_audioSource.clip = clip;
        //     _audioSourceHit.PlayOneShot(clip);
        // }
    }
    private void OnChangeFireRate(bool IsChange)
    {
        if (IsChange)
        {
            _bulletType = PoolType.Projectile_Bullet_BBQ;
        }
        else
        {
            _bulletType = PoolType.Projectile_Bullet_Norman;
        }
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

        // Phát âm thanh khi súng ng?ng xoay n?u ?ang n?p ??n
        if (isBarrelSpinning)
        {
            _audioSource.clip = weaponInfo.AudioEndBarrel;
            _audioSource.Play();
            isBarrelSpinning = false;
        }
    }

    private IEnumerator DecreaseRotationSpeed()
    {
        while (currentRotationSpeed > weaponInfo.MinSpeedRotaBarrel)
        {
            currentRotationSpeed -= (weaponInfo.MaxSpeedRotaBarrel / weaponInfo.TimeMinSpeed) * Time.deltaTime;
            if (currentRotationSpeed < weaponInfo.MinSpeedRotaBarrel)
            {
                currentRotationSpeed = weaponInfo.MinSpeedRotaBarrel;
            }
            yield return null;
        }
    }

    private Transform FindPointedTransform()
    {
        // var minCrossHairDistance = float.MaxValue;
        Transform pointedTransform = null;

        // var bots = BotManager.Instance.BotNetworks;
        // foreach (var bot in bots.Where(bot => bot != null && !bot.IsDead))
        // {
        //     CheckFireAssistCheckPos(bot.FireAssistCheckPos, ref minCrossHairDistance, ref pointedTransform);
        // }
        //
        // var rewards = RewardManager.Instance.RewardNetworks;
        // foreach (var reward in rewards.Where(reward => reward != null && !reward.IsCollected))
        // {
        //     CheckFireAssistCheckPos(reward.FireAssistCheckPos, ref minCrossHairDistance, ref pointedTransform);
        // }

        return pointedTransform;
    }

    private void CheckFireAssistCheckPos(List<Transform> fireAssistCheckPos, ref float minCrossHairDistance, ref Transform pointedTransform)
    {
        foreach (var checkPoint in fireAssistCheckPos)
        {
            var checkPosition = checkPoint.position;

            if (!SatisfyAutoFireCondition(checkPosition, out var crossHairDistance) ||
                crossHairDistance > minCrossHairDistance) continue;

            minCrossHairDistance = crossHairDistance;
            pointedTransform = checkPoint;
        }
    }

    [SerializeField] private float radius = 33f;
    private const float ReferenceWidth = 887;

    private bool SatisfyAutoFireCondition(Vector3 target, out float distance)
    {
        var viewPosition = _camera.WorldToScreenPoint(target);
        if (viewPosition.z < 0)
        {
            distance = float.MaxValue;
            return false;
        }
        viewPosition.x -= Screen.width / 2f;
        viewPosition.y -= Screen.height / 2f;

        viewPosition *= ReferenceWidth / Screen.width;

        distance = Mathf.Sqrt(viewPosition.x * viewPosition.x + viewPosition.y * viewPosition.y);
        return distance < radius && IsClearShot(_cameraTransform.position, target);
    }

    private bool IsClearShot(Vector3 origin, Vector3 target)
    {
        var distance = Vector3.Distance(origin, target);
        var ray = new Ray(origin, target - origin);
        return !Physics.Raycast(ray, out _, distance, botLayerMask | armorBossLayerMask | rewardLayerMask | groundLayerMask);
    }

    // Thêm ph??ng th?c d?ng âm thanh b?n

    private void StopShootingSound()
    {
        if (_audioSource.isPlaying && _audioSource.clip == weaponInfo.audioClip)
        {
            _audioSource.Stop();
        }
    }

    // Thêm ph??ng th?c nh?n AnimationEvent
    public void AnimationAudioEvent()
    {
        // Th?c hi?n hành ??ng khi s? ki?n AnimationAudioEvent ???c g?i
        //Debug.Log("AnimationAudioEvent called");
    }

    // Thêm hàm rung l?c camera
    private IEnumerator ShakeCamera(float duration, float magnitude)
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


    private void PlayGunEffect()
    {
        foreach (ParticleSystem fireEffect in _fireEffect)
        {
            if (fireEffect != null && !fireEffect.isPlaying)
            {
                fireEffect.Play();
            }
        }
    }

    private void StopGunEffect()
    {
        foreach (ParticleSystem fireEffect in _fireEffect)
        {
            if (fireEffect != null && fireEffect.isPlaying)
            {
                fireEffect.Stop();
            }
        }
    }
}
