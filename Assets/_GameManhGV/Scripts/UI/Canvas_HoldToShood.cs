using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canvas_HoldToShood : UICanvas
{
    [SerializeField] private BotZombieNor_Start botTuTorial;
    public void event_HoldToShoot()
    {
        CloseDirectly();
        UIManager.Instance.OpenUI<Canvas_GamePlay>();
        
        botTuTorial.CallOnTakeDamage(0);
    }
}