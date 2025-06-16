using System.Collections;
using static GameConstants;
using UnityEngine;

public class BossZomOgre_Attack : StateBase<ZomAllState,BossNetwork>
{
    [SerializeField] Transform _posSpawnBullet;
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
        if(_coroutineAttack!= null)
            StopCoroutine(_coroutineAttack);
    }

    IEnumerator IEAttack()
    {
        int randomSkill = Random.Range(0, 2);
        thisBotNetworks.RotaToPlayerMain();
        if (randomSkill == 0)
        {
            //.9fspawn + setup,  2.3f rời bullet cho bay, 1.05 = end skill0
            thisBotNetworks.ChangeAnimAndType("Attack",0);
            yield return new WaitForSeconds(.9f);
            BulletBloodBlobZom bulletBloodBlobZom = SimplePool.Spawn<BulletBloodBlobZom>(PoolType.BulletBloodBlobZom, _posSpawnBullet.position, Quaternion.identity);
            bulletBloodBlobZom.SetupSpawn(_posSpawnBullet, 1);
            yield return new WaitForSeconds(2.3f);
            bulletBloodBlobZom.OnInit(LocalPlayer.Instance.GetPosLocalPlayer());
            yield return new WaitForSeconds(1.5f);
        }
        else
        {
            //2.5 all ,1.3 = spawn skill đập đất
            thisBotNetworks.ChangeAnimAndType("Attack",1);
            
            Vector3 posSelf = transform.position;
            Vector3 posLocalPlayer = LocalPlayer.Instance._localPlayer.position;
            posLocalPlayer.y = 0;
            Vector3 direction = (posLocalPlayer - posSelf).normalized;
            float spacing = Vector3.Distance(posSelf, posLocalPlayer) / 3; // Khoảng cách giữa các điểm
            
            yield return new WaitForSeconds(1.3f);

            for (int i = 1; i < 4; i++)
            {
                Vector3 spawnPos = posSelf + direction * (spacing * i);
                Effect effect = SimplePool.Spawn<Effect>(PoolType.vfx_ExplosionGround, spawnPos, Quaternion.identity);
                effect.OnInit();
                yield return new WaitForSeconds(.4f);
                if (i == 3)
                { }
                //TODO:Attack player
            }
        }

        if (Random.Range(0, 10) > 5)
            thisStateController.ChangeState(ZomAllState.Move);
        else
            thisStateController.ChangeState(ZomAllState.Idle);
    }
}