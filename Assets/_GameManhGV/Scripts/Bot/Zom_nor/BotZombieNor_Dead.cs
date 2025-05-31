using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotZombieNor_Dead : BaseState<BotZomNorState>
{
    [SerializeField] private bool tutorialReload;
    public override void EnterState()
    {
        int animType = Random.Range(0, 2);
        thisBotNetwork.SetAnimAndType("Dead",animType);
        isDoneState = false;
        thisBotNetwork.ActiveFalseDetectors();
        
        if(tutorialReload)
            Invoke(nameof(TutorialReload),.5f);

        Invoke(nameof(thisBotNetwork.OnDespawn), 5f);
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