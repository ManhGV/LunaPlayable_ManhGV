using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossZomChainSaw_NetWork : BossNetwork
{
    public override void SetActiveDetectors(bool _active, int _skillType)
    {
        base.SetActiveDetectors(_active, _skillType);
        detectors[_skillType].SetActive(_active);
    }
}