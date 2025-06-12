using static GameConstants;
using UnityEngine;
using UnityEngine.Serialization;

public class BossZomSwatNetword : BossNetwork
{
    [FormerlySerializedAs("_bossZomSwat")]
    [Header("Bắn rocket 1 lần ngã")] 
    [SerializeField] private BossZomSwat_State bossZomSwatState;
    [SerializeField] BallisticArmor[] arrBallisticArmor;
    public bool canExplosionArmor;
    
    public void ExplosinArrmor()
    {
        if (canExplosionArmor)
        {
            foreach (BallisticArmor VARIABLE in arrBallisticArmor)
                VARIABLE.ExplosionArmor();
            bossZomSwatState.ChangeState(ZomAllState.Stun_2);
        }
    }
}