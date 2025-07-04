// GameConstants.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConstants
{
    public enum BotType
    {
        None = 0,
        poinZomLeoTreo = 1,
        BotZomNorNurse = PoolType.BotZomNorNurse,
        BotZomNorPatient = PoolType.BotZomNorPatient,
        BotZomNorGirl = PoolType.BotZomNorGirl,
        BossZomSwat = PoolType.Boss_Zom_Swat,
        BotZomNorNurseTranNha = PoolType.BotZomNorNurseTranNha,
        BotZomNorGirlTranNha = PoolType.BotZomNorGirlTranNha,
    }
    public enum PoolType
    {
        None = 0,
        Projectile_Bullet_Norman = 1,
        Projectile_Bullet_BBQ = 2,
        vfx_BloodEffectZom = 3,
        vfx_ConcreteImpact = 4,
        BotZomNorPatient = 5,
        BotZomNorNurse = 6,
        BotZomNorGirl = 7,
        Boss_Zom_Swat = 8,
        bullet_Rocket = 9,
        vfx_ExplosionRocket = 10,
        vfx_ExplosionZombieNor = 11,
        vfx_ShootGift = 12,
        BotZomNorNurseTranNha = 13,
        BotZomNorGirlTranNha = 14,
        AudioChild = 15,
    }
    public enum RewardType
    {
        None,
        ChangeWeapon81,
    }
    public enum GameState
    {
        Playing,
        Paused,
        CutScene,
        EndGame,
    }
}
