using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossZomChainSaw_NetWork : BossNetwork
{
    [SerializeField] AudioSource _audioSourceIdle;
    protected override void Awake()
    {
        base.Awake();
        Invoke(nameof(PlayLoopAudioVoice), 3.8f);
    }

    private void OnEnable()
    {
        if(AudioManager.Instance != null)
            PlayAudioVoice(0, 1, false);
    }

    public void PlayLoopAudioVoice()
    {
        _audioSourceIdle.Play();
    }

    public override void SetActiveDetectors(bool _active, int _skillType)
    {
        base.SetActiveDetectors(_active, _skillType);
        detectors[_skillType].SetActive(_active);
    }
}