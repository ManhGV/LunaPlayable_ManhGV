using System;
using System.Collections.Generic;
using UnityEngine;
using static GameConstants;

[CreateAssetMenu(fileName = "ConfigGame", menuName = "ScriptableObjects/ConfigGame", order = 1)]
public class ConfigGame : ScriptableObject
{
    public FightRound fightRound;
}
[Serializable]
public class FightRound
{
    public BotConfig[] botConfigs;
    public RewardConfig[] rewardConfig;
}

[Serializable]
public class BotConfig
{
    [Header("Thông tin Bot")]
    [Tooltip("Chọn loại Bot")]
    public PoolType botType;

    public List<SpawmQuantity> spawmQuantities = new List<SpawmQuantity>();
}

[Serializable]
public class SpawmQuantity
{
    public PoinSpawnbot PoinSpawnbot;
 
    [Tooltip("Số lượng bot sinh ra")]
    public int botQuantity;

    [Tooltip("Thời gian Delay giữa các lần thả bot")]
    public float botDelaySpawn;
    
    [Tooltip("Thời gian delay bao nhiêu giây từ đầu trận đấu để bắt đầu thả bot")]
    public float WaitToSpawn;
}

[Serializable]
public class RewardConfig
{
    [Header("Thông tin phần thưởng")]
    [Tooltip("Chọn loại phần thưởng")]
    public RewardType rewardType;

    [Tooltip("Thời gian delay từ đầu trận đấu để bắt đầu thả phần thưởng")]
    public float WaitToSpawn;
}