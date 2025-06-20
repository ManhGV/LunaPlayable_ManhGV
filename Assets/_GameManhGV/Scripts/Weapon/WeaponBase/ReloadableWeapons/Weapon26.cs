using UnityEngine;
using static GameConstants;

public class Weapon26 : ReloadableWeapons
{
    protected override void Awake()
    {
        base.Awake();
        UIManager.Instance.GetUI<Canvas_GamePlay>().Init();
        Invoke(nameof(OnInit), .1f);
    }

    protected override void AddAnimationClips()
    {
        base.AddAnimationClips();
        if (_animation != null && weaponInfo != null)
        {
            _animation.AddClip(weaponInfo.Fire, "Fire");
            _animation.AddClip(weaponInfo.Idle, "Idle");
            _animation.AddClip(weaponInfo._reloadAnimIn, "ReloadIn");
            _animation.AddClip(weaponInfo._reloadAnimOn, "ReloadOn");
            _animation.AddClip(weaponInfo._reloadAnimOut, "ReloadOut");
        }
    }

    protected override void LogicPlayGun()
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
                    EventManager.Invoke(EventName.UpdateBulletCount, _currentBulletCount);
                }
                PlayGunEffect(); // Kích hoạt hiệu ứng nổ súng
            }
        }
    }

    protected override void FireFromMuzzle(Transform muzzle, Vector3 forward)
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

            if (IsInLayerIndex(hit.collider.gameObject,0)) //bot
            {
                var takeDamageController = hit.transform.gameObject.GetComponent<ITakeDamage>();
                if (takeDamageController == null)
                {
                    takeDamageController = hit.transform.root.gameObject.GetComponent<ITakeDamage>();
                }
                if (takeDamageController != null) takeDamageController.TakeDamage(damageInfo);
                typeEffect = GameConstants.PoolType.vfx_BloodEffectZom;
                EventManager.Invoke(EventName.OnCheckBotTakeDamage, true);
                // Debug.Log(damageType.ToString() + " " + weaponInfo.damage + " " + hit.collider.name);
            }
            else if (IsInLayerIndex(hit.collider.gameObject,1)) //weakpoint
            {
                var takeDamageController = hit.transform.gameObject.GetComponent<ITakeDamage>();
                if (takeDamageController == null)
                    takeDamageController = hit.transform.root.gameObject.GetComponent<ITakeDamage>();
                if (takeDamageController != null)
                {
                    EventManager.Invoke(EventName.OnCheckBotTakeDamage, true);
                    takeDamageController.TakeDamage(damageInfo);
                }
                typeEffect = GameConstants.PoolType.vfx_ShootGift;
            }
            else if (IsInLayerIndex(hit.collider.gameObject,2)) //reward
            {
                var rewardController = hit.transform.gameObject.GetComponent<IReward>();
                if (rewardController == null)
                {
                    rewardController = hit.transform.root.gameObject.GetComponent<IReward>();
                }
                if (rewardController != null) rewardController.TakeCollect(weaponInfo.damage);
                typeEffect = GameConstants.PoolType.vfx_ShootGift;
            }
            else if (IsInLayerIndex(hit.collider.gameObject,3)) //ground
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
            else if (IsInLayerIndex(hit.collider.gameObject,4))//gas
            {
                var takeDamageController1 = hit.transform.gameObject.GetComponent<ITakeDamage>();
                if (takeDamageController1 == null)
                    takeDamageController1 = hit.transform.root.gameObject.GetComponent<ITakeDamage>();
                if (takeDamageController1 != null) 
                    takeDamageController1.TakeDamage(damageInfo);
                EventManager.Invoke(EventName.OnCheckBotTakeDamage, true);
            }else if (IsInLayerIndex(hit.collider.gameObject, 5))//chạm vào word
            {
                typeEffect = GameConstants.PoolType.vfx_Wood;
            }
            else
            {
                EventManager.Invoke(EventName.OnCheckBotTakeDamage, false);
            }
            // Tạo hiệu ứng va chạm
            SimplePool.Spawn<Effect>(typeEffect, hit.point, Quaternion.identity).OnInit();
        }
    }
}