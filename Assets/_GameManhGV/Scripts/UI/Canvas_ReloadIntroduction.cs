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
        ReloadableWeapons reloadableWeapons = WeaponBase.Instance as ReloadableWeapons;
        bulletCountCurrent.text = reloadableWeapons.CurrentBulletCount.ToString();
        bulletCountDefault.text = reloadableWeapons.DefaultBulletCount.ToString();
    }
}