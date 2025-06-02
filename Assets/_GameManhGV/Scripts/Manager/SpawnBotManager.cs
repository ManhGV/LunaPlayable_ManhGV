using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBotManager : Singleton<SpawnBotManager>
{
    [SerializeField] private PathManager _pathManager;
    [SerializeField] private ConfigGame dataBotSpawn;
    [SerializeField] private List<BotNetwork> botInScene = new List<BotNetwork>();
    
    public int CountBotInScene => botInScene.Count;
    public int AllBotsToSpawn;
    
    protected override void Awake()
    {
        base.Awake();

        foreach (BotConfig VARIABLE in dataBotSpawn.fightRound.botConfigs)
            AllBotsToSpawn += VARIABLE.botQuantity;
        
    }

    public void SpawnBot()
    {
        foreach(BotConfig botConfig in dataBotSpawn.fightRound.botConfigs)
            StartCoroutine(IEOnSpawnBot(botConfig));
    }

    IEnumerator IEOnSpawnBot(BotConfig _botConfig)
    {
        yield return new WaitForSeconds(_botConfig.WaitToSpawn);
        GameConstants.PoolType poolType;
        WayPoint wayPoint;

        for (int i = 0; i < _botConfig.botQuantity; i++)
        {
            
            wayPoint = _pathManager.GetWayPoint(GameConstants.PoolType.None);
            poolType = (GameConstants.PoolType)_botConfig.botType;
            BotNetwork botNetwork = SimplePool.Spawn<BotNetwork>(poolType, wayPoint.WayPoints[0].position, Quaternion.identity);
            botNetwork.Init(wayPoint);
            botInScene.Add(botNetwork);

            yield return new WaitForSeconds(_botConfig.botDelaySpawn);
        }
    }

    public void RemoveBotDead(BotNetwork _botNetwork)
    {
        botInScene.Remove(_botNetwork);
        //TODO: Add logic tính điểm còn lại
    }
}
