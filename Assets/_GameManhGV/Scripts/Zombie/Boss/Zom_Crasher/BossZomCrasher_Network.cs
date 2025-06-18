using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossZomCrasher_Network : BossNetwork
{
    public override void SetActiveDetectors(bool _active, int _skillType)
    {
        base.SetActiveDetectors(_active, _skillType);
        if (_skillType == 0)
            detectors[0].SetActive(_active);
        else if(_skillType == 1)
        {
            detectors[1].SetActive(_active);
            detectors[2].SetActive(_active);
        }
    }
}