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
        BossSwat,
    }
    public enum PoolType
    {
        None = 0,
        Projectile_Bullet_Norman = 1,
        Projectile_Bullet_Norma = 2,
        vfx_BloodEffectZom = 3,
        vfx_ConcreteImpact = 4,
        Boss_Zom_Swat = 5,
        
    }
    public enum RewardType
    {
        None,
        RapidFire,
        ChangeMachineGun,
        ChangeRocket,
    }
    public enum GameState
    {
        Playing,
        Paused,
    }
}
