using System.Collections;
using static GameConstants;
using UnityEngine;

public class BossZomOgre_Attack : StateBase<ZomAllState,BossZomOgre_Network>
{
    [SerializeField] Transform _posSpawnBullet;
    private Coroutine _coroutineAttack;
    private BulletBloodBlobZom bulletBloodBlobZom;
    public override void EnterState()
    {
        _coroutineAttack = StartCoroutine(IEAttack());
    }

    public override void UpdateState()
    {
        
    }

    public override void ExitState()
    {
        if (_coroutineAttack != null)
        {
            thisBotNetworks.SetActiveAllDetectors(false);
            StopCoroutine(_coroutineAttack);
            if(bulletBloodBlobZom != null && bulletBloodBlobZom.gameObject.activeSelf)
                bulletBloodBlobZom.OnDead();
        }
    }

    IEnumerator IEAttack()
    {
        int randomSkill = Random.Range(0, 2);
        thisBotNetworks.RotaToPlayerMain();
        if (randomSkill == 0)
        {
            //.9fspawn + setup,  2.3f rời bullet cho bay, 1.05 = end skill0
            thisBotNetworks.ChangeAnimAndType("Attack",0);
            thisBotNetworks.SetActiveDetectors(true,0);
            yield return new WaitForSeconds(.9f);
            thisBotNetworks.SetFloatAnim("Attack1_AnimScale",.7f);
            bulletBloodBlobZom = SimplePool.Spawn<BulletBloodBlobZom>(PoolType.BulletBloodBlobZom, _posSpawnBullet.position, Quaternion.identity);
            bulletBloodBlobZom.SetupSpawn(_posSpawnBullet, 1);
            yield return new WaitForSeconds(1.43f);
            thisBotNetworks.SetFloatAnim("Attack1_AnimScale",1);
            thisBotNetworks.SetActiveDetectors(false,0);
            yield return new WaitForSeconds(1.3f);
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
            
            yield return new WaitForSeconds(.2f);
            thisBotNetworks.SetActiveDetectors(true,1);
            thisBotNetworks.SetFloatAnim("Attack2_AnimScale",0.6f);
            yield return new WaitForSeconds(1.33f);
            thisBotNetworks.SetActiveDetectors(false,1);
            thisBotNetworks.SetFloatAnim("Attack2_AnimScale",1);
            yield return new WaitForSeconds(0.3f);

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
        _coroutineAttack = null;
        if (Random.Range(0, 10) > 5)
            thisStateController.ChangeState(ZomAllState.Move);
        else
            thisStateController.ChangeState(ZomAllState.Idle);
    }
}