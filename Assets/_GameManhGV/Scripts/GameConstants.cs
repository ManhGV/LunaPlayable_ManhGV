// GameConstants.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConstants
{
#if UNITY_EDITOR
    public enum BotType
    {
        None = 0,
        poinZomLeoTreo = 1,
        BotZomNorNurse = PoolType.BotZomNorNurse,
        BotZomNorPatient = PoolType.BotZomNorPatient,
        BotZomNorGirl = PoolType.BotZomNorGirl,
        BotZomNorNurseTranNha = PoolType.BotZomNorNurseTranNha,
        BotZomNorGirlTranNha = PoolType.BotZomNorGirlTranNha,
        BossZomSwat = PoolType.Boss_Zom_Swat,
        BossZomRevenant = PoolType.BossZom_Revenant,
        BossZomChainSaw = PoolType.BossZomChainSaw,
        BossZomCrasher = PoolType.BossZomCrasher,
        BossZomThrower = PoolType.BossZomThrower,
        BossZomHulk = PoolType.BossZomHulk,
        bossZomOgre = PoolType.BossZomOgre,
        bot_zom_Builder_Mix = PoolType.bot_zom_Builder_Mix,
        bot_zom_Woman = PoolType.bot_zom_Woman,
        bot_zom_Hight_Mix = PoolType.bot_zom_Hight_Mix,
        bot_zom_suicider = PoolType.bot_zom_suicider,
        bot_zom_spider_suicide = PoolType.bot_zom_spider_suicide,
        bot_zom_bomber = PoolType.bot_zom_bomber,
        bot_zom_dog_suicide = PoolType.bot_zom_dog_suicide,
    }

    public enum EffectType
    {
        None = 0,
        BloodEffectZom = PoolType.vfx_BloodEffectZom,
        ConcreteImpact = PoolType.vfx_ConcreteImpact,
        ExplosionRocket = PoolType.vfx_ExplosionRocket,
        ExplosionZombieNor = PoolType.vfx_ExplosionZombieNor,
        ShootGift = PoolType.vfx_ShootGift,
        ExplosionWeapon112 = PoolType.vfx_explosionWeapon112,
        ElectricLine = PoolType.vfx_electricLine,
        vfx_ExplosionGround = PoolType.vfx_ExplosionGround,
        Wood = PoolType.vfx_Wood,
        ElectricHit = PoolType.vfx_ElectricHit,
    }

    public enum ProjecctileZombie
    {
        BulletFireZom = PoolType.BulletFireZom,
        BulletRockZombie = PoolType.BulletRockZombie,
        GroundCrashZom = PoolType.GroundCrashZom,
        BulletBloodBlobZom = PoolType.BulletBloodBlobZom,
    }

    public enum ProjecttilePlayer
    {
        Projectile_Bullet_Norman = PoolType.Projectile_Bullet_Norman,
        Projectile_Bullet_BBQ = PoolType.Projectile_Bullet_BBQ,
        bullet_Rocket = PoolType.bullet_Rocket,
    }

    public enum LinhTinh
    {
        AudioChild = PoolType.AudioChild,
    }
#endif

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
        vfx_explosionWeapon112 = 16,
        vfx_electricLine = 17,
        Projectile_Weapon112 = 18,
        BossZom_Revenant = 19,
        vfx_ExplosionGround = 20,
        BulletFireZom=21,
        BossZomChainSaw = 22,
        BossZomCrasher = 23,
        BossZomThrower = 24,
        BulletRockZombie = 25,
        GroundCrashZom = 26,
        BossZomHulk = 27,
        BossZomOgre = 28,
        BulletBloodBlobZom = 29,
        vfx_Wood = 30,
        vfx_ElectricHit = 31,
        bot_zom_Builder_Mix = 32,
        bot_zom_Woman = 33,
        bot_zom_Hight_Mix = 34,
        bot_zom_suicider = 35,
        bot_zom_spider_suicide = 36,
        bot_zom_bomber = 37,
        bot_zom_dog_suicide = 38,
    }
    
    public enum ZomAllState
    {
        Start,
        Idle,
        Move,
        Jump,
        JumpPunch,
        Attack,
        Scream,
        Stun_1,
        Stun_2,
        Dead,
        DeadExplosion,
    }
    
    public enum RewardType
    {
        None,
        ChangeWeapon81,
        ChangeWeapon112,
    }
    public enum GameState
    {
        Playing,
        Paused,
        CutScene,
        EndGame,
    }
}
