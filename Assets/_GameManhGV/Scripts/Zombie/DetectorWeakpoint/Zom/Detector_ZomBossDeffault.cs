using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector_ZomBossDeffault : DetectorBase
{
    [Header("Setup Stun")]
    [Tooltip("Kiểu stun cho boss có 2 stun trở lên")][SerializeField] private int _stunType;
    [SerializeField] private BossNetwork _bossNetwork;
    
    public override void OnDead()
    {
        base.OnDead();
        _bossNetwork.OnEventDetectorDead(_stunType);
    }
}