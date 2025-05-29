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
        bulletCountCurrent.text = WeaponController.Instance.CurrentBulletCount.ToString();
        bulletCountDefault.text = WeaponController.Instance.DefaultBulletCount.ToString();
    }
}