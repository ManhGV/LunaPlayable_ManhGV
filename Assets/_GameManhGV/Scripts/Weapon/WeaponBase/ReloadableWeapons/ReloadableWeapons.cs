using System.Collections;
using UnityEngine;

public class ReloadableWeapons : WeaponBase
{
    [Header("Shooting")]
    protected float _timeSinceLastShoot = 0f; // Thời gian từ lần bắn cuối cùng
    [SerializeField] protected int _currentBulletCount; // Số lượng đạn hiện tại trong băng
    protected bool isShooting = false; // Trạng thái đang bắn
    protected Coroutine shootingCoroutine;
    
    [Header("Reload")]
    private bool _isReloading = false; // Trạng thái đang nạp đạn
    private Coroutine _reloadCoroutine;
    [SerializeField] private AudioClip _reloadFastAudio;
    
    [Header("Lằng nhằng")]
    [SerializeField] protected Animation _animation;
    [SerializeField] private GameConstants.PoolType _bulletType;  // Loại đạn sẽ được bắn
    
    [Header("Muzzle")]
    [SerializeField] protected Transform _muzzleTrans_1;
    public int CurrentBulletCount => _currentBulletCount;
    public int DefaultBulletCount => weaponInfo.bulletCount;
    
    protected override void Awake()
    {
        base.Awake();
        _currentBulletCount = weaponInfo.bulletCount; // Khởi tạo số lượng đạn
    }

    protected override void Update()
    {
        base.Update();
        if (_isReloading)
            return;

        _timeSinceLastShoot += Time.deltaTime;

        if (readyShoot)
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

    private void LogicStopGun()
    {
        if (isShooting)
        {
            StopShootingSound();
            isShooting = false;
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

        if (_timeSinceLastShoot >= weaponInfo.FireRate)
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
        shootingCoroutine = null;
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
                print("Bot");
            }
            else if (IsInLayerIndex(hit.collider.gameObject,1))
            {
                print("Detected");
                // Detector detector = hit.transform.gameObject.GetComponent<Detector>();
                // if (detector != null)
                //     detector.SetHealthImage(damageInfo.damage);
                //
                // BallisticArmor ballisticArmor = hit.transform.gameObject.GetComponent<BallisticArmor>();
                // if (ballisticArmor != null)
                //     ballisticArmor.TakeDamage(damageInfo.damage);
                //
                // damageInfo.damage /= 3;
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
                OxygenTanks oxygenTanks = null;
                oxygenTanks = hit.transform.gameObject.GetComponent<OxygenTanks>();
                if (oxygenTanks != null)
                    oxygenTanks.Explosion();
                typeEffect = GameConstants.PoolType.vfx_ShootGift;
            }

            // Tạo hiệu ứng va chạm
            SimplePool.Spawn<Effect>(typeEffect, hit.point, Quaternion.identity).OnInit();
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
}
