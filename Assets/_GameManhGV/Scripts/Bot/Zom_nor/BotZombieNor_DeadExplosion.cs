using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 0 = trước
// 1 = phải
// 2 = trái
// 3 = sau
public class BotZombieNor_DeadExplosion : BaseState<BotZomNorState>
{
    [SerializeField] private bool tutorialReload;
    [SerializeField] private float timerDelayDespawn;
    public override void EnterState()
    {
        isDoneState = false;
        int animType = thisBotNetwork.GetNearestDirection();

        timerDelayDespawn = 5f;
        
        thisBotNetwork.SetAnimAndType("DeadExplosion", animType);
        
        if(tutorialReload)
            Invoke(nameof(TutorialReload),.5f);
    }

    public override void UpdateState()
    {
        if(isDoneState)
            return;
        
        timerDelayDespawn -= Time.deltaTime;
        if (timerDelayDespawn <= 0)
        {
            if(!thisBotNetwork.isBotTutorial)
                thisBotNetwork.OnDespawn();
            isDoneState = true;
        }
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
        SpawnBotManager.Instance.SpawnBot();
        WeaponController weaponController = WeaponController.Instance;
        tutorialReload = weaponController.instructReload;
        if (!tutorialReload)
        {
            tutorialReload = true;
            weaponController.InstructReload();
        }
    }
}