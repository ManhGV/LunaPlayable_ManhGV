using System.Collections;
using static GameConstants;
using UnityEngine;

public class BossZomHulk_Attack : StateBase<ZomAllState,BossZomHulk_Netword>
{
    [SerializeField] private Transform _posSpawmRock;
    private Coroutine _coroutineAttack;
    public override void EnterState()
    {
        _coroutineAttack = StartCoroutine(IEAttack());
    }

    public override void UpdateState()
    {
        
    }

    public override void ExitState()
    {
        if(_coroutineAttack != null)
            StopCoroutine(_coroutineAttack);
    }

    IEnumerator IEAttack()
    {
        thisBotNetworks.RotaToPlayerMain();
        thisBotNetworks.ChangeAnim("Attack");
        yield return new WaitForSeconds(1.26f);
        BulletRockZombie bulletRockZombie = SimplePool.Spawn<BulletRockZombie>(PoolType.BulletRockZombie, _posSpawmRock.position, Quaternion.identity);
        bulletRockZombie.SetupSpawn(_posSpawmRock,1.45f);
        thisBotNetworks.PlayAudioVoice(3, 1, false);
        yield return new WaitForSeconds(2.74f);
        thisBotNetworks.PlayAudioVoice(4, 1, false);
        yield return new WaitForSeconds(.5f);
        bulletRockZombie.OnInit(LocalPlayer.Instance.GetPosLocalPlayer());
        yield return new WaitForSeconds(1.05f);
        
        int random = Random.Range(0, 4);
        if(random == 0)
            thisStateController.ChangeState(ZomAllState.Idle);
        else if(random == 1)
            thisStateController.ChangeState(ZomAllState.Move);
        else if(random == 2)
            thisStateController.ChangeState(ZomAllState.Jump);
        else
            thisStateController.ChangeState(ZomAllState.JumpPunch);
    }
}