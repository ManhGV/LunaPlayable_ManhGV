using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Canvas_ReloadIntroduction : UICanvas
{
    [SerializeField] Text bulletCountCurrent;
    [SerializeField] Text bulletCountDefault;

    private void OnEnable()
    {
        print(WeaponBase.Instance.CurrentBulletCount);
        bulletCountCurrent.text = WeaponBase.Instance.CurrentBulletCount.ToString();
        bulletCountDefault.text = WeaponBase.Instance.DefaultBulletCount.ToString();
    }
}