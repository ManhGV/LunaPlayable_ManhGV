using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canvas_HoldToShood : UICanvas
{
    public void event_HoldToShoot()
    {
        CloseDirectly();
        UIManager.Instance.OpenUI<Canvas_GamePlay>();
        SpawnBotManager.Instance.SpawnBot();
        
    }
}