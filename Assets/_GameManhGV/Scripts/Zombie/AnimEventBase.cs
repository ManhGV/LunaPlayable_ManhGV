using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEventBase : MonoBehaviour
{
    [SerializeField] AudioClip[] audioFootstep;

    public void PlayFootStep_1(float _volume) => AudioManager.Instance.PlaySound(audioFootstep[0], _volume);
    public void PlayFootStep_2(float _volume) => AudioManager.Instance.PlaySound(audioFootstep[1], _volume);
}
