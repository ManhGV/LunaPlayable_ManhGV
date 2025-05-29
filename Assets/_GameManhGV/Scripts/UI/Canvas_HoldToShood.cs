using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canvas_HoldToShood : UICanvas
{
    public void event_HoldToShoot()
    {
        CloseDirectly();
        UIManager.Instance.OpenUI<Canvas_GamePlay>();
        if (ParameterManagers.IsIngameGUI)
        {
            //UIManager.Instance.OpenUI<Canvas_GamePlay>().btn_HoldToShoot();
        }
        else
        {
            //UIManager.Instance.OpenUI<Canvas_MainMenu>().btn_HoldToShoot();
        }
    }
}