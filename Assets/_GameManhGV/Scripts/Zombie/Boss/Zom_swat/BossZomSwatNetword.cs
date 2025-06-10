using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossZomSwatNetword : BossNetwork
{
    [Header("Bắn rocket 1 lần ngã")] 
    [SerializeField] private BossZomSwat _bossZomSwat;
    [SerializeField] BallisticArmor[] arrBallisticArmor;
    public bool canExplosionArmor;
    
    public void ExplosinArrmor()
    {
        if (canExplosionArmor)
        {
            foreach (BallisticArmor VARIABLE in arrBallisticArmor)
                VARIABLE.ExplosionArmor();
            _bossZomSwat.ChangeState(BossZomSwatState.Stun_2);
        }
    }
}