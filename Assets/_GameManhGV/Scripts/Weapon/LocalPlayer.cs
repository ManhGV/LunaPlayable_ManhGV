using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayer : Singleton<LocalPlayer>
{
    public Transform  _localPlayer;

    public Vector3 GetPosLocalPlayer()
    {
        return _localPlayer.position;
    }
    
    public Transform GetTranformPlayer()
    {
        return _localPlayer;
    }
}
