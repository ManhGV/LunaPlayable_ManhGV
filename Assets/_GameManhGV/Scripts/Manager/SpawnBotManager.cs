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
    
    [Header("BossZomSwat")]
    [SerializeField] GameObject _bossZomSwat;
    [SerializeField] bool canCallBossZomSwat;
    
    protected override void Awake()
    {
        base.Awake();

        canCallBossZomSwat = true;
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

    private void Update()
    {
        if (canCallBossZomSwat && _currentBot <= 1)
        {
            canCallBossZomSwat = false;
            StartCoroutine(IECallBossZomSwat(1.8f));
            StartCoroutine(IEOnTutorialRocket());
        }
    }

    private IEnumerator IEOnTutorialRocket()
    {
        yield return new WaitForSeconds(26f);
        RocketController.Instance.InstructRocket();
    }

    public IEnumerator IECallBossZomSwat(float time)
    {
        yield return new WaitForSeconds(time);
        GameManager.Instance.StartCutScene();
        PlayerHP.Instance.ClearListDamage();
        CutSceneCam.Instance.MoveFromAToB(0,1,2.5f,70f);
        yield return new WaitForSeconds(1.5f);
        _bossZomSwat.SetActive(true);
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
            
            poolType = (GameConstants.PoolType)_botConfig.botType;
            if ((int)poolType < 12)
                wayPoint = _pathManager.GetWayPoint(GameConstants.BotType.None);
            else
                wayPoint = _pathManager.GetWayPoint(GameConstants.BotType.poinZomLeoTreo);
            BotNetwork botNetwork = SimplePool.Spawn<BotNetwork>(poolType, wayPoint.WayPoints[0].position, Quaternion.Euler(0, 180, 0));
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
