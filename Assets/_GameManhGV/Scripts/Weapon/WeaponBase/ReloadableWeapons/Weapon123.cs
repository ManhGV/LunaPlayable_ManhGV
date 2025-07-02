using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon123 : WeaponRotationMuzzle
{
    [SerializeField] Transform _muzzleTrans_2;
    
    protected override void Shoot()
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
        FireFromMuzzle(_muzzleTrans_2, forward);

        _animation.Play("Fire");
        _animation["Fire"].speed = 2.0f;
        _audioSource.clip = weaponInfo.audioClip;
        _audioSource.Play();

        UICrosshairItem.Instance.Expand_Crosshair(15);

        PlayGunEffect();
    }

    private bool canCancreateVfx = true;
    protected override void FireFromMuzzle(Transform muzzle, Vector3 forward)
    {
        canCancreateVfx = !canCancreateVfx;
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
            if(canCancreateVfx)
                SimplePool.Spawn<EffectVfx>(typeEffect, hit.point, Quaternion.identity).OnInit();
        }
        EventManager.Invoke(EventName.OnCheckBotTakeDamage, CheckRayCast);
    }
}