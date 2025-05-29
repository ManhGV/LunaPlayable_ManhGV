// GameConstants.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConstants
{
    public enum BotType
    {
        None,
        zombieNorsuit,
        zombieNornurse,
        zombieNornursemix,
        zombieNorwormanmix,
        zombieNorwormanmixClim,
        BossChainsaw,
    }
    public enum PoolType
    {
        Projectile_Bullet_Norman = 0,
        Projectile_Bullet_Norma = 1,
        vfx_BloodEffectZom = 2,
        vfx_ConcreteImpact = 3,
    }
    public enum RewardType
    {
        None,
        RapidFire,
        ChangeMachineGun,
        ChangeRocket,
    }
}
