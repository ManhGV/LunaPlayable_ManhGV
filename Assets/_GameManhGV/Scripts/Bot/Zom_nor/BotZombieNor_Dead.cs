using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotZombieNor_Dead : BaseState<BotZomNorState>
{
    [SerializeField] private bool tutorialReload;
    public override void EnterState()
    {
        thisBotNetwork.ChangeAnim("Dead");
        isDoneState = false;
        thisBotNetwork.ActiveFalseDetectors();
        
        if(tutorialReload)
            Invoke(nameof(TutorialReload),.5f);
    }

    public override void UpdateState()
    {
        
    }

    public override void ExitState()
    {
        
    }

    public override BotZomNorState GetNextState()
    {
        return StateKey;
    }

    public void TutorialReload()
    {
        WeaponController weaponController = WeaponController.Instance;
        tutorialReload = weaponController.instructReload;
        if (!tutorialReload)
        {
            tutorialReload = true;
            weaponController.InstructReload();
        }
    }
}