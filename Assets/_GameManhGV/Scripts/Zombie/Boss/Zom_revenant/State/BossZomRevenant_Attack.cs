using System.Collections;
using UnityEngine;
using static GameConstants;

public class BossZomRevenant_Attack : StateBase<ZomAllState, BossZomRevenant_Netword>
{
    [SerializeField] private ParticleSystem _fireBallFake;
    bool CanChangeState;
    Coroutine _attackCoroutine;
    private float posYthis;
    private int attackType;
    public override void EnterState()
    {
        posYthis = transform.position.y;
        CanChangeState = false;
        thisBotNetworks.RotaToPlayerMain();
        
        if (thisBotNetworks.DistanceToPlayermain() <= 9.74)
        {
            attackType = 01;
            _attackCoroutine = StartCoroutine(IEAttackType_01(Random.Range(3, 5)));
            //TODO: Skill trocj 2 cánh
        }
        else if (posYthis > 0)
        {
            //TODO:ném lửa
            _attackCoroutine = StartCoroutine(IEAttackType_2());
        }
        else
        {
            //TODO: đấm đất
            _attackCoroutine = StartCoroutine(IEAttackType_3());
            // timer = 3.1f;
        }
    }
    public override void UpdateState()
    {
        //print(timer);
        if (CanChangeState)
        {
            int random;
            if (attackType == 01)
            {
                random = Random.Range(0, 2);
                if (random == 0)
                    thisStateController.ChangeState(ZomAllState.Move);
                else
                    thisStateController.ChangeState(ZomAllState.Jump);
            }
            else
            {
                random = Random.Range(0, 3);

                if (random == 0)
                    thisStateController.ChangeState(ZomAllState.Idle);
                else if (random == 1 && posYthis <= 0)
                    thisStateController.ChangeState(ZomAllState.Move);
                else
                    thisStateController.ChangeState(ZomAllState.Jump);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
            thisStateController.ChangeState(ZomAllState.Stun_1);
        //TODO: DoneState thì => random move, idle, jump
    }

    public override void ExitState()
    {
        if (_attackCoroutine != null)
        {
            StopCoroutine(_attackCoroutine);
            thisBotNetworks.SetFloatAnim("SpeedAtkAnim", 1);
            thisBotNetworks.SetActiveAllDetectors(false);
        }
    }


    /// <param name="_countAttack">Số lần vẩy cánh</param>
    /// <returns></returns>
    IEnumerator IEAttackType_01(int _countAttack)
    {
        for (int i = 0; i < _countAttack; i++)
        {
            if (i % 2 == 0)
            {

                thisBotNetworks.ChangeAnimAndType("Attack", 0);
                yield return new WaitForSeconds(.25f);
                thisBotNetworks.SetFloatAnim("SpeedAtkAnim", .3f);
                thisBotNetworks.SetActiveDetectors(true,0);
                yield return new WaitForSeconds(1.3f);
                thisBotNetworks.SetActiveDetectors(false,0);
                thisBotNetworks.SetFloatAnim("SpeedAtkAnim", 1);

                yield return new WaitForSeconds(2.2f); //2.26 là thực anim
                //TODO:Attack player
            }
            else
            {
                thisBotNetworks.ChangeAnimAndType("Attack", 1);
                yield return new WaitForSeconds(.25f);
                thisBotNetworks.SetFloatAnim("SpeedAtkAnim", .3f);
                thisBotNetworks.SetActiveDetectors(true,1);
                yield return new WaitForSeconds(1.3f);
                thisBotNetworks.SetActiveDetectors(false,1);
                thisBotNetworks.SetFloatAnim("SpeedAtkAnim", 1);

                yield return new WaitForSeconds(2.2f);
                //TODO:Attack player
            }
        }

        _attackCoroutine = null;
        CanChangeState = true;
    }

    IEnumerator IEAttackType_2()
    {
        thisBotNetworks.ChangeAnimAndType("Attack", 2);
        yield return new WaitForSeconds(.25f);
        _fireBallFake.Play();
        yield return new WaitForSeconds(.5f);
        _fireBallFake.Stop();
        yield return new WaitForSeconds(.2f);
        BulletFireZombie bulletZomBase = SimplePool.Spawn<BulletFireZombie>(PoolType.BulletFireZom, _fireBallFake.transform.position, Quaternion.identity);
        bulletZomBase.OnInit(LocalPlayer.Instance.GetPosLocalPlayer());
        yield return new WaitForSeconds(1.19f);
        _attackCoroutine = null;
        CanChangeState = true;
    }

    IEnumerator IEAttackType_3()
    {
        thisBotNetworks.ChangeAnimAndType("Attack", 3);
        Vector3 posSelf = transform.position;
        Vector3 posLocalPlayer = LocalPlayer.Instance._localPlayer.position;
        posLocalPlayer.y = 0;
        Vector3 direction = (posLocalPlayer - posSelf).normalized;
        float spacing = Vector3.Distance(posSelf, posLocalPlayer) / 3; // Khoảng cách giữa các điểm

        yield return new WaitForSeconds(.4f);
        thisBotNetworks.SetFloatAnim("SpeedAtkAnim", .28f);
        thisBotNetworks.SetActiveDetectors(true, 2);
        yield return new WaitForSeconds(1.78f);
        thisBotNetworks.SetActiveDetectors(false, 2);
        thisBotNetworks.SetFloatAnim("SpeedAtkAnim", 1);
        yield return new WaitForSeconds(.3f);

        for (int i = 1; i < 4; i++)
        {
            Vector3 spawnPos = posSelf + direction * (spacing * i);
            Effect effect = SimplePool.Spawn<Effect>(PoolType.vfx_ExplosionGround, spawnPos, Quaternion.identity);
            effect.OnInit();
            yield return new WaitForSeconds(.34f);
            if (i == 3)
            { }
            //TODO:Attack player
        }

        yield return new WaitForSeconds(1f);
        _attackCoroutine = null;
        CanChangeState = true;
    }

}