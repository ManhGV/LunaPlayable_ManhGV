using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Android;
using static GameConstants;

public class BossZomRevenant_Attack : StateBase<ZomAllState, BossZomRevenant_Netword>
{
    private int AttackType;
    bool CanChangeState;
    Coroutine _attackCoroutine;
    private float posYthis;
    public override void EnterState()
    {
        posYthis = transform.position.y;
        CanChangeState = false;
        thisBotNetworks.RotaToPlayerMain();
        if (thisBotNetworks.DistanceToPlayermain() <= 9.74)
        {
            //TODO: Skill trocj 2 cánh
            AttackType = 1;
            _attackCoroutine = StartCoroutine(IEAttackType_1(Random.Range(1, 5)));
        }
        else if (posYthis > 0)
        {
            //TODO:ném lửa
            AttackType = 2;
            _attackCoroutine = StartCoroutine(IEAttackType_2());
        }
        else
        {
            //TODO: đấm đất
            AttackType = 3;
            _attackCoroutine = StartCoroutine(IEAttackType_3());
            // timer = 3.1f;
        }
    }

    public override void UpdateState()
    {
        if (CanChangeState)
        {
            // int random = Random.Range(0, 3);
            // if (random == 0)
            //     thisStateController.ChangeState(ZomAllState.Idle);
            // else if (random == 1 && posYthis <= 0)
            // else
            //     thisStateController.ChangeState(ZomAllState.Jump);
                thisStateController.ChangeState(ZomAllState.Move);
        }
        
        if(Input.GetKeyDown(KeyCode.Space))
            thisStateController.ChangeState(ZomAllState.Stun_1);
        //TODO: DoneState thì => random move, idle, jump
    }

    public override void ExitState()
    {
        if (_attackCoroutine != null)
            StopCoroutine(_attackCoroutine);
    }

    /// <param name="_countAttack">Số lần vẩy cánh</param>
    /// <returns></returns>
    IEnumerator IEAttackType_1(int _countAttack)
    {
        for (int i = 0; i < _countAttack; i++)
        {
            if (i % 2 == 0)
            {
                thisBotNetworks.PlayAnim("AtkTentacle_L");
                yield return new WaitForSeconds(2.65f); //2.26 là thực anim
                //TODO:Attack player
            }
            else
            {
                thisBotNetworks.PlayAnim("AtkTentacle_R");
                yield return new WaitForSeconds(2.65f);
                //TODO:Attack player
            }
        }

        _attackCoroutine = null;
        CanChangeState = true;
    }

    IEnumerator IEAttackType_2()
    {
        thisBotNetworks.PlayAnim("AtkFire");
        yield return new WaitForSeconds(.95f);
        print("create fire");
        yield return new WaitForSeconds(1.19f);
        _attackCoroutine = null;
        CanChangeState = true;
    }

    IEnumerator IEAttackType_3()
    {
        thisBotNetworks.PlayAnim("AtkGround");
        Vector3 posSelf = transform.position;
        Vector3 posLocalPlayer = LocalPlayer.Instance._localPlayer.position;
        posLocalPlayer.y = 0;
        Vector3 direction = (posLocalPlayer - posSelf).normalized;
        float spacing = Vector3.Distance(posSelf, posLocalPlayer) / 3; // Khoảng cách giữa các điểm
        yield return new WaitForSeconds(.54f);
        
        for (int i = 1; i < 4; i++)
        {
            Vector3 spawnPos = posSelf + direction * (spacing * i);
            yield return new WaitForSeconds(.5f);
            Effect effect = SimplePool.Spawn<Effect>(PoolType.vfx_ExplosionGround, spawnPos, Quaternion.identity);
            effect.OnInit();
            if(i==3)
            {}
                //TODO:Attack player
        }
        
        yield return new WaitForSeconds(1.12f);
        _attackCoroutine = null;
        CanChangeState = true;
    }
    
}