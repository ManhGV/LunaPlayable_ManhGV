using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBotManager : Singleton<SpawnBotManager>
{
    private PathManager _pathManager;
    [SerializeField] private ConfigGame dataBotSpawn;
    [SerializeField] private List<BotNetwork> botInScene = new List<BotNetwork>();
    public int CountBotInScene => botInScene.Count;
    public int allBotsToSpawn => botInScene.Count;
    private void Start()
    {
        _pathManager = PathManager.Instance;
        SpawnBot(dataBotSpawn);
    }

    void SpawnBot(ConfigGame _configGame)
    {
        foreach(BotConfig botConfig in _configGame.fightRound.botConfigs)
            StartCoroutine(IEOnSpawnBot(botConfig));
    }

    IEnumerator IEOnSpawnBot(BotConfig _botConfig)
    {
        yield return new WaitForSeconds(_botConfig.WaitToSpawn);
        GameConstants.PoolType poolType;
        WayPoint wayPoint;

        for (int i = 0; i < _botConfig.botQuantity; i++)
        {
            
            wayPoint = _pathManager.GetWayPoint((GameConstants.PoolType)_botConfig.botType);
            poolType = (GameConstants.PoolType)_botConfig.botType;
            BotNetwork botNetwork = SimplePool.Spawn<BotNetwork>(poolType, wayPoint.WayPoints[0].position, Quaternion.identity);
            botNetwork.Init(wayPoint);
            botInScene.Add(botNetwork);

            yield return new WaitForSeconds(_botConfig.botDelaySpawn);
        }
    }
}
