using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEvent : Singleton<WeaponEvent>
{
    //public  WeaponInfo[] weaponInfo;
    public  Transform WeaponDefault;
    public Transform WeaponChange;
    //public float[] DefaultFireRate;
    public bool IsChangeBullet;

    public float PosWeaponDefaultEnd;
    public float PosWeaponChangeEnd;
    public float transitionTime = 1f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))    
            OnChangeMachineGun(true);
    }

    private void OnEnable()
    {
        EventManager.AddListener<bool>(EventName.OnChangeWeapon, OnChangeMachineGun);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener<bool>(EventName.OnChangeWeapon, OnChangeMachineGun);
    }


    private void OnChangeMachineGun(bool IsChange)
    {
        if (IsChange)
        {
            StartCoroutine(OnChangeWeapon());
        }
        else
        {
            WeaponDefault.gameObject.SetActive(true);
            WeaponChange.gameObject.SetActive(false);
        }

    }

    private IEnumerator OnChangeWeapon()
    {
        // Di chuyển WeaponDefault từ vị trí A đến B theo trục Z
        EventManager.Invoke(EventName.UpdateBulletCount, 0);
        float elapsedTime = 0f;
        Vector3 startPosition = WeaponDefault.localPosition;
        Vector3 endPosition = new Vector3(startPosition.x, startPosition.y, PosWeaponDefaultEnd);

        while (elapsedTime < transitionTime)
        {
            WeaponDefault.localPosition = Vector3.Lerp(startPosition, endPosition, elapsedTime / transitionTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        WeaponDefault.localPosition = endPosition;

        // Ẩn WeaponDefault
        WeaponDefault.gameObject.SetActive(false);
        // Hiển thị WeaponChange
        WeaponChange.gameObject.SetActive(true);
        if (IsChangeBullet)
        {
            EventManager.Invoke(EventName.OnChangeFireRate, true);
        }
        // Di chuyển WeaponChange từ vị trí M đến N theo trục Y
        float elapsedTime2 = 0f;
        Vector3 startPosition2 = WeaponChange.localPosition;
        Vector3 endPosition2 = new Vector3(startPosition2.x, PosWeaponChangeEnd, startPosition2.z);

        while (elapsedTime2 < transitionTime)
        {
            WeaponChange.localPosition = Vector3.Lerp(startPosition2, endPosition2, elapsedTime2 / transitionTime);
            elapsedTime2 += Time.deltaTime;
            yield return null;
        }
        WeaponChange.localPosition = endPosition2;
        UIManager.Instance.GetUI<Canvas_GamePlay>().UpdateBulletChangeWeapon();
    }
}
