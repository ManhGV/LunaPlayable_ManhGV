using System.Collections;
using UnityEngine;

public class BotZombieNor_Dead : BaseState<BotZomNorState>
{
    [SerializeField] private bool tutorialReload = false;
    [SerializeField] private Transform _posCenter;
    int animType;
    public override void EnterState()
    {
        animType = Random.Range(0, 2);
        thisBotNetwork.SetAnimAndType("Dead", animType);
        isDoneState = false;
        thisBotNetwork.ActiveFalseDetectors();

        if (animType == 0)
            StartCoroutine(IEDelayAnimAndDespawn(4f));
        else if (animType == 1)
            StartCoroutine(IEDelayAnimAndDespawn(5f));

        if (tutorialReload)
            Invoke(nameof(TutorialReload), .5f);
    }

    public override void UpdateState()
    {
    }

    private IEnumerator IEDelayAnimAndDespawn(float _timerDelay)
    {
        yield return new WaitForSeconds(_timerDelay);
        if (animType == 1)
        {
            Effect explosionPoolZomNor = SimplePool.Spawn<Effect>(GameConstants.PoolType.vfx_ExplosionZombieNor, _posCenter.position, Quaternion.identity);
            explosionPoolZomNor.OnInit();
        }
        thisBotNetwork.OnDespawn();
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
        SpawnBotManager spawnBot = SpawnBotManager.Instance;
        spawnBot.SpawnBot();
        spawnBot.ActiveGiftWeapon81(5f);
        Weapon26 weapon26 = (Weapon26)WeaponBase.Instance;
        weapon26.InstructReload();
    }
}