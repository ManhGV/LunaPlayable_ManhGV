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
    public override void EnterState()
    {
        isDoneState = false;
        int animType = thisBotNetwork.GetNearestDirection();
        
        thisBotNetwork.SetAnimAndType("DeadExplosion", animType);
        
        if(tutorialReload)
            Invoke(nameof(TutorialReload),.5f);

        StartCoroutine(IEDelayAnimAndDespawn(3f));
        thisBotNetwork.ActiveFalseDetectors();
    }

    public override void UpdateState()
    {
        
    }

    public override void ExitState()
    {
    }
    
    private IEnumerator IEDelayAnimAndDespawn(float _timerDelay)
    {
        yield return new WaitForSeconds(_timerDelay);
        thisBotNetwork.OnDespawn();
    }
    public override BotZomNorState GetNextState()
    {
        return StateKey;
    }
    
    public void TutorialReload()
    {
        SpawnBotManager spawnBot = SpawnBotManager.Instance;
        spawnBot.SpawnBot();
        spawnBot.ActiveGiftWeapon81(5f);
        Weapon26 weapon26 = (Weapon26)WeaponBase.Instance;
        weapon26.InstructReload();
    }
}