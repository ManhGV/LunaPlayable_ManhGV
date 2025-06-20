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

    private BulletParabolZombie bulletParabolZombie;
    private CraskGroundRockZom craskGroundRockZom;
    public override void ExitState()
    {
        if (_attackCoroutine != null)
        {
            StopCoroutine(_attackCoroutine);
            bulletParabolZombie.OnDespawn();
            craskGroundRockZom.OnDespawn();
            thisBotNetworks.SetFloatAnim("animAttackSpeed", 1);
        }
    }
    
    IEnumerator IEAttack(int _animType)
    {
        if (_animType == 0)
        {
            thisBotNetworks.ChangeAnimAndType("Attack", 0);
            yield return new WaitForSeconds(1.2f);
            thisBotNetworks.PlayAudioVoice(2,1,false);
            yield return new WaitForSeconds(.6f);
            thisBotNetworks.PlayAudioVoice(2,1,false);
            yield return new WaitForSeconds(.2f);
            bulletParabolZombie = SimplePool.Spawn<BulletParabolZombie>(PoolType.BulletRockZombie, postSpawn.position, Quaternion.identity);
            bulletParabolZombie.SetupSpawn(postSpawn,1f);
            yield return new WaitForSeconds(.8f);
            vfxRock.Play();
            yield return new WaitForSeconds(1.7f);
            thisBotNetworks.PlayAudioVoice(3,1,false);
            yield return new WaitForSeconds(.5f);
            bulletParabolZombie.TF.parent = null;
            bulletParabolZombie.OnInit(LocalPlayer.Instance.GetPosLocalPlayer());
            yield return new WaitForSeconds(2.2f);
        }
        else if (_animType == 1)
        {
            thisBotNetworks.ChangeAnimAndType("Attack", 1);
            yield return new WaitForSeconds(0.6f);
            thisBotNetworks.SetActiveAllDetectors(true);
            thisBotNetworks.SetFloatAnim("animAttackSpeed", 0.2f);
            yield return new WaitForSeconds(1.5f);
            thisBotNetworks.SetFloatAnim("animAttackSpeed", 1);
            thisBotNetworks.SetActiveAllDetectors(false);
            
            yield return new WaitForSeconds(0.7f);
            craskGroundRockZom = SimplePool.Spawn<CraskGroundRockZom>(PoolType.GroundCrashZom, postSpawn.position, Quaternion.identity);
            craskGroundRockZom.OnInit(transform.position,LocalPlayer.Instance.GetPosLocalPlayer());
        }
        else
            thisStateController.ChangeState(ZomAllState.Idle);

        _attackCoroutine = null;
        _doneAttack = true;
    }
}