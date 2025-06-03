using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBotManager : Singleton<SpawnBotManager>
{
    [SerializeField] private PathManager _pathManager;
    [SerializeField] private ConfigGame dataBotSpawn;
    [SerializeField] private List<BotNetwork> botInScene = new List<BotNetwork>();

    public int KillBot => AllBotsToSpawn - _currentBot;
    public int _currentBot;
    public int AllBotsToSpawn;
    
    [Header("Gift")]
    [SerializeField] GameObject giftWeapon81;
    
    protected override void Awake()
    {
        base.Awake();

        foreach (BotConfig VARIABLE in dataBotSpawn.fightRound.botConfigs)
            AllBotsToSpawn += VARIABLE.botQuantity;
        AllBotsToSpawn += 2;
        _currentBot = AllBotsToSpawn;
        
    }

    private void Start()
    {
        float progress = (float) KillBot / (float)AllBotsToSpawn;
        EventManager.Invoke(EventName.UpdateGameProcess, progress);
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
        _currentBot--;
        botInScene.Remove(_botNetwork);
        float progress = (float) KillBot / (float)AllBotsToSpawn;
        EventManager.Invoke(EventName.UpdateGameProcess, progress);
    }
    
    public void ActiveGiftWeapon81(float _timer)
    {
        StartCoroutine(IEDelaySpawnGiftWeapon81(_timer));
    }

    private IEnumerator IEDelaySpawnGiftWeapon81(float _timer)
    {
        yield return new WaitForSeconds(_timer);
        giftWeapon81.SetActive(true);
    }
}
