using UnityEngine;
using static GameConstants;

public class Weapon26 : ReloadableWeapons
{
    [Header("Tutorial")]
    public bool instructReload = false;

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
                    if (!instructReload && _currentBulletCount <= 3)
                        InstructReload();

                    EventManager.Invoke(EventName.UpdateBulletCount, _currentBulletCount);
                }
                PlayGunEffect(); // Kích hoạt hiệu ứng nổ súng
            }
        }
    }

    public void InstructReload()
    {
        //nếu chưa hướng dẫn reload thì hướng dẫn reload
        if (!instructReload)
        {
            _currentBulletCount = 3;
            EventManager.Invoke(EventName.UpdateBulletCount, _currentBulletCount);
            instructReload = true;
            EventManager.Invoke(EventName.InstructReload, true);
        }
    }
}