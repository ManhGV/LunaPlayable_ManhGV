using System.Collections;
using UnityEngine;
using static GameConstants;

public class BossZomThrower_Attack : StateBase<ZomAllState, BossZomThrower_Netword>
{
    [SerializeField] private Transform postSpawn;
    [SerializeField] private ParticleSystem vfxRock;
    Coroutine _attackCoroutine;
    private bool _doneAttack;
    public override void EnterState()
    {
        _doneAttack = false;
        thisBotNetworks.RotaToPlayerMain();
        StartCoroutine(IEAttack(Random.Range(0,2)));
    }

    public override void UpdateState()
    {
        if (_doneAttack)
        {
            if (Random.Range(0, 10) <= 7)
                thisStateController.ChangeState(ZomAllState.Jump);
            else
            {
                if (Random.Range(0, 10) < 6)
                    thisStateController.ChangeState(ZomAllState.Move);
                else
                    thisStateController.ChangeState(ZomAllState.Idle);
                
            }
        }
    }

    public override void ExitState()
    {
        if(_attackCoroutine != null)
            StopCoroutine(_attackCoroutine);
    }

    IEnumerator IEAttack(int _animType)
    {
        if (_animType == 0)
        {
            thisBotNetworks.ChangeAnimAndType("Attack", 0);
            yield return new WaitForSeconds(2f);
            BulletParabolZombie bulletParabolZombie = SimplePool.Spawn<BulletParabolZombie>(PoolType.BulletRockZombie, postSpawn.position, Quaternion.identity);
            bulletParabolZombie.SetupSpawn(postSpawn);
            yield return new WaitForSeconds(.8f);
            vfxRock.Play();
            yield return new WaitForSeconds(2.2f);
            bulletParabolZombie.TF.parent = null;
            bulletParabolZombie.OnInit(LocalPlayer.Instance.GetPosLocalPlayer());
            yield return new WaitForSeconds(2.2f);
        }
        else if (_animType == 1)
        {
            thisBotNetworks.ChangeAnimAndType("Attack", 1);
            CraskGroundRockZom craskGroundRockZom = SimplePool.Spawn<CraskGroundRockZom>(PoolType.GroundCrashZom, postSpawn.position, Quaternion.identity);
            yield return new WaitForSeconds(1.2f);
            craskGroundRockZom.OnInit(transform.position,LocalPlayer.Instance.GetPosLocalPlayer());
            yield return new WaitForSeconds(.4f);
        }
        else
        {
            thisStateController.ChangeState(ZomAllState.Idle);
        }

        _doneAttack = true;
    }
}