using System.Collections;
using UnityEngine;

public class BotZombieNor_Dead : BaseState<BotZomNorState>
{
    [SerializeField] private bool tutorialReload;
    [SerializeField] private Transform _posCenter;
    private Coroutine _coroutineDead;
    int animType;
    public override void EnterState()
    {
        animType = Random.Range(0, 2);
        thisBotNetwork.SetAnimAndType("Dead", animType);
        isDoneState = false;
        thisBotNetwork.ActiveFalseDetectors();

        if (animType == 0)
            _coroutineDead = StartCoroutine(IEDelayAnimAndDespawn(4f));
        else if (animType == 1)
            _coroutineDead = StartCoroutine(IEDelayAnimAndDespawn(5f));

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
            ExplosionPoolZomNor explosionPoolZomNor = SimplePool.Spawn<ExplosionPoolZomNor>(GameConstants.PoolType.vfx_ExplosionZombieNor, _posCenter.position, Quaternion.identity);
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