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
}