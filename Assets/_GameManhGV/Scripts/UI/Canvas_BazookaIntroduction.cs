using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Canvas_BazookaIntroduction : UICanvas
{
    public void btn_RocketFake()
    {
        UIManager.Instance.GetUI<Canvas_GamePlay>().Btn_Rocket();
    }
}