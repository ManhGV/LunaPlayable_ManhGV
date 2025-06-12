using System.Collections;
using static GameConstants;
using UnityEngine;
// 0 = trước
// 1 = phải
// 2 = trái
// 3 = sau
public class BotZombieNor_DeadExplosion : StateBase<ZomAllState, BotNetwork>
{
    [SerializeField] private GameObject Detector;
    [SerializeField] private bool tutorialReload;
    public override void EnterState()
    {
        int animType = thisBotNetworks.GetNearestDirection();
        
        thisBotNetworks.PlayAudioVoice(Random.Range(0,4),1, true);
        thisBotNetworks.ChangeAnimAndType("DeadExplosion", animType);
        
        if(tutorialReload)
            Invoke(nameof(TutorialReload),.5f);

        StartCoroutine(IEDelayAnimAndDespawn(3f));
        if(Detector != null)
            Detector.SetActive(false);
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
        thisBotNetworks.OnDespawn();
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