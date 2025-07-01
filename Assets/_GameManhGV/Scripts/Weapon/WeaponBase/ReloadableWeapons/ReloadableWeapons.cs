using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class ReloadableWeapons : WeaponBase
{
    [Header("Độ đỏ đầu nòng")]
    [SerializeField] protected ParticleSystem[] _vfxSmokeMuzzle; // Hiệu ứng khói từ nòng súng
    private bool _isSmokePlaying = false;
    [SerializeField] protected Transform _muzzleCenter; // Nơi bắn đạn từ nòng súng
    [SerializeField] protected Material _materialGun;
    [Tooltip("Độ đỏ tối đa của nòng")] protected float _maxGlowColor = 6f;
    [Tooltip("Nhiệt độ nòng")] [SerializeField] protected float _temperatureCurrent;
    [Tooltip("Nhiệt độ để nòng đạt độ đỏ tối đa")] [SerializeField] protected float _temperatureToColorMax = 2f;
    
    [Header("Shooting")]
    protected float _timeSinceLastShoot = 0f; // Thời gian từ lần bắn cuối cùng
    [SerializeField] protected int _currentBulletCount; // Số lượng đạn hiện tại trong băng
    protected Coroutine shootingCoroutine;
    
    [Header("Reload")]
    private bool _isReloading = false; // Trạng thái đang nạp đạn
    private Coroutine _reloadCoroutine;
    [SerializeField] private AudioClip _reloadFastAudio;
    
    [Header("Lằng nhằng")]
    [SerializeField] protected Animation _animation;
    [SerializeField] protected GameConstants.PoolType _bulletType;  // Loại đạn sẽ được bắn
    
    [Header("Muzzle")]
    [SerializeField] protected Transform _muzzleTrans_1;
    public int CurrentBulletCount => _currentBulletCount;
    public int DefaultBulletCount => weaponInfo.bulletCount;
    
    protected override void Awake()
    {
        base.Awake();
        _temperatureCurrent = 0;
        _currentBulletCount = weaponInfo.bulletCount; // Khởi tạo số lượng đạn
        _materialGun.SetVector("_Glow", Vector4.zero);
    }
    protected override void Start()
    {
        if (WeaponBase.Instance is ReloadableWeapons reloadableWeapons)
        {
            _bulletType = reloadableWeapons._bulletType;
            weaponInfo.FireRate = reloadableWeapons.weaponInfo.FireRate;
        }
        base.Start();
    }
    private void FixedUpdate()
    {
        _materialGun.SetVector("_Muzzle", new Vector4(_muzzleCenter.position.x,_muzzleCenter.position.y, _muzzleCenter.position.z, 0f));
    }

    protected override void Update()
    {
        base.Update();
        if (_isReloading)
        {
            UpOrDowTemperature(false);
            _materialGun.SetVector("_Muzzle", new Vector4(_muzzleCenter.position.x,_muzzleCenter.position.y, _muzzleCenter.position.z, 0f));
            return;
        }
        
        _timeSinceLastShoot += Time.deltaTime;

        if (_isHoldScreen)
            LogicPlayGun();
        else
            LogicStopGun();
    }

    protected override void OnInit()
    {
        base.OnInit();
        EventManager.Invoke(EventName.UpdateBulletCount, _currentBulletCount);
        EventManager.Invoke(EventName.UpdateBulletCountDefault, weaponInfo.bulletCount);
    }

    protected virtual void LogicStopGun()
    {
        UpOrDowTemperature(false);
        StopShootingSound();
        if (shootingCoroutine != null)
        {
            StopCoroutine(shootingCoroutine);
            shootingCoroutine = null;
        }
        StopGunEffect(); // Dừng hiệu ứng nổ súng
    }

    protected virtual void LogicPlayGun()
    {
        _materialGun.SetVector("_Muzzle", new Vector4(_muzzleCenter.position.x,_muzzleCenter.position.y, _muzzleCenter.position.z, 0f));
        UICrosshairItem.Instance.Narrow_Crosshair();
        if (_timeSinceLastShoot >= weaponInfo.FireRate)
        {
            if (_currentBulletCount <= 0 && !weaponInfo.infiniteBullet)
                OnReload();
            else
            {
                UpOrDowTemperature(true);
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

    protected void UpOrDowTemperature(bool _isUp)
    {
        if (_isUp)
        {
            if (_isSmokePlaying)
            {
                foreach (ParticleSystem _vfx in _vfxSmokeMuzzle)
                    _vfx.Stop();

                _isSmokePlaying = false;
            }
            if (Math.Abs(_temperatureCurrent + _temperatureToColorMax) < .01f)
                return;
            
            if(_temperatureCurrent < _temperatureToColorMax)
                _temperatureCurrent += 6f * Time.deltaTime;
            else
                _temperatureCurrent = _temperatureToColorMax;
            _materialGun.SetVector("_Glow", new Vector4(0, .4f, _temperatureCurrent / _temperatureToColorMax * _maxGlowColor, 0));
        }
        else
        {
            if (_temperatureCurrent == 0)
                return;
            if (!_isSmokePlaying)
            {
                foreach (ParticleSystem _vfx in _vfxSmokeMuzzle)
                    _vfx.Play();

                _isSmokePlaying = true;
            }
            
            if (_temperatureCurrent >= 0)
                _temperatureCurrent -= .8f * Time.deltaTime;
            else
            {
                if (_isSmokePlaying)
                {
                    foreach (ParticleSystem _vfx in _vfxSmokeMuzzle)
                        _vfx.Stop();

                    _isSmokePlaying = false;
                }
                _temperatureCurrent = 0;
            }
            _materialGun.SetVector("_Glow", new Vector4(0, .4f, _temperatureCurrent / _temperatureToColorMax * _maxGlowColor, 0));
        }
    }

    protected virtual void Shoot()
    {
        if (this == null || _cameraTransform == null) return;
        
        Vector3 forward= _cameraTransform.forward;
        
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

    protected virtual void FireFromMuzzle(Transform muzzle, Vector3 forward)
    {
        var shotRotation = Quaternion.Euler(Random.insideUnitCircle * weaponInfo.inaccuracy) * forward;
        var ray = new Ray(_cameraTransform.position, shotRotation);
        BulletTrail bullet = SimplePool.Spawn<BulletTrail>(_bulletType, muzzle.position, muzzle.rotation);
        Vector3 posGizmod = GizmodCaculatorPointShoot();
        bullet.Init((posGizmod - muzzle.position).normalized, posGizmod);

        bool CheckRayCast = Physics.Raycast(ray, out var hit, Mathf.Infinity, CombinedLayerMask());
        GameConstants.PoolType typeEffect = GameConstants.PoolType.vfx_ConcreteImpact;
        if (CheckRayCast)
        {
            DamageInfo damageInfo = new DamageInfo()
            {
                damageType = DamageType.Normal,
                damage = weaponInfo.damage,
                name = hit.collider.name,
            };

            if (IsInLayerIndex(hit.collider.gameObject,0))
            {
                var takeDamageController = hit.transform.gameObject.GetComponent<ITakeDamage>();
                if (takeDamageController == null)
                {
                    takeDamageController = hit.transform.root.gameObject.GetComponent<ITakeDamage>();
                }
                if (takeDamageController != null) takeDamageController.TakeDamage(damageInfo);
                typeEffect = GameConstants.PoolType.vfx_BloodEffectZom;
                // Debug.Log(damageType.ToString() + " " + weaponInfo.damage + " " + hit.collider.name);
            }
            else if (IsInLayerIndex(hit.collider.gameObject,1))
            {
                var takeDamageController = hit.transform.gameObject.GetComponent<ITakeDamage>();
                if (takeDamageController == null)
                {
                    takeDamageController = hit.transform.root.gameObject.GetComponent<ITakeDamage>();
                }
                if (takeDamageController != null) takeDamageController.TakeDamage(damageInfo);
                typeEffect = GameConstants.PoolType.vfx_ShootGift;
            }
            else if (IsInLayerIndex(hit.collider.gameObject,2))
            {
                var rewardController = hit.transform.gameObject.GetComponent<IReward>();
                if (rewardController == null)
                {
                    rewardController = hit.transform.root.gameObject.GetComponent<IReward>();
                }
                if (rewardController != null) rewardController.TakeCollect(weaponInfo.damage);
                typeEffect = GameConstants.PoolType.vfx_ShootGift;
            }
            else if (IsInLayerIndex(hit.collider.gameObject,3))
            {
                var takeDamageController1 = hit.transform.gameObject.GetComponent<ITakeDamage>();
                if (takeDamageController1 == null)
                {
                    takeDamageController1 = hit.transform.root.gameObject.GetComponent<ITakeDamage>();
                }
                if (takeDamageController1 != null) takeDamageController1.TakeDamage(damageInfo);
                //Debug.Log(damageType.ToString() + " " + weaponInfo.damage + " " + hit.collider.name);
                typeEffect = GameConstants.PoolType.vfx_ConcreteImpact;
            }
            else if (IsInLayerIndex(hit.collider.gameObject,4))
            {
                var takeDamageController1 = hit.transform.gameObject.GetComponent<ITakeDamage>();
                if (takeDamageController1 == null)
                    takeDamageController1 = hit.transform.root.gameObject.GetComponent<ITakeDamage>();
                
                if (takeDamageController1 != null) 
                    takeDamageController1.TakeDamage(damageInfo);
                typeEffect = GameConstants.PoolType.vfx_ShootGift;
            }

            // Tạo hiệu ứng va chạm
            SimplePool.Spawn<EffectVfx>(typeEffect, hit.point, Quaternion.identity).OnInit();
        }
        EventManager.Invoke(EventName.OnCheckBotTakeDamage, CheckRayCast);
    }
    

    #region Reload
    public void OnReload()
    {
        if (_isReloading || _currentBulletCount >= weaponInfo.bulletCount)
            return;
        
        if (_reloadCoroutine != null)
            StopCoroutine(_reloadCoroutine);
        
        _reloadCoroutine = StartCoroutine(Reload());
        
        _isReloading = true;
        StopGunEffect();
        UICrosshairItem.Instance.ResetCorosshair();
    }

    public void OnReloadFast(int _PlusAmount)
    {
        if (_reloadCoroutine != null)
            StopCoroutine(_reloadCoroutine);
        
        _isReloading = false;
        _audioSource.PlayOneShot(_reloadFastAudio);
        _animation.Play("ReloadOut");
        _currentBulletCount = weaponInfo.bulletCount + _PlusAmount;
        UICrosshairItem.Instance.ResetCorosshair();
        EventManager.Invoke(EventName.UpdateBulletCount, _currentBulletCount);
    }

    private IEnumerator Reload()
    {
        UIManager.Instance.GetUI<Canvas_GamePlay>().ActiveReloadFast();
        
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
        GameManager.Instance.DontSlomotionTimeScale();
        EventManager.Invoke(EventName.UpdateBulletCount, _currentBulletCount);
    }
    #endregion

    public override void ChangeFireRate(GameConstants.PoolType typeBulletGift, float _fireRate)
    {
        weaponInfo.FireRate = _fireRate;
        _bulletType = typeBulletGift;
    }
}
