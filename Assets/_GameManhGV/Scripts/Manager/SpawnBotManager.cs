using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SpawnBotManager : Singleton<SpawnBotManager>
{
    [SerializeField] private PathManager _pathManager;
    [SerializeField] private ConfigGame dataBotSpawn;
    [SerializeField] private List<ZombieBase> botInScene = new List<ZombieBase>();
    [SerializeField] private List<BotZombieNor_Start> botHaveStart = new List<BotZombieNor_Start>();

    public int KillBot => AllBotsToSpawn - _currentBot;
    public int _currentBot;
    public int AllBotsToSpawn;
    
    [Header("Gift")]
    [SerializeField] GameObject giftWeapon81;
    
    [Header("BossZom")]
    [SerializeField] GameObject _boss;
    [SerializeField] bool canCallBoss;
    
    protected override void Awake()
    {
        base.Awake();
        foreach (BotConfig VARIABLE in dataBotSpawn.fightRound.botConfigs)
            AllBotsToSpawn += VARIABLE.botQuantity;
        AllBotsToSpawn += 6;
        _currentBot = AllBotsToSpawn;
    }

    private void Start()
    {
        float progress = (float) KillBot / (float)AllBotsToSpawn;
        EventManager.Invoke(EventName.UpdateGameProcess, progress);
    }

    // private void Update()
    // {
    //     if (canCallBoss && _currentBot <= 1)
    //     {
    //         canCallBoss = false;
    //     }
    // }

    public IEnumerator IECallBossZom(float time)
    {
        yield return new WaitForSeconds(time);
        PlayerHP.Instance.ClearListDamage();
        yield return new WaitForSeconds(1.5f);
        GameManager.Instance.ActiveSoundCombat();
        _boss.SetActive(true);
    }

    public void CallAllBotAttack()
    {
        foreach (BotZombieNor_Start VARIABLE in botHaveStart)
            VARIABLE.CallOnTakeDamage();
    }
    
    public void SpawnBot()
    {
        StartCoroutine(IECallBossZom(20f));
        StartCoroutine(IEDelaySpawnGiftWeapon81(6.5f));
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
            
            poolType = _botConfig.botType;
            wayPoint = _pathManager.GetWayPoint(_botConfig.PoinSpawnbot);
            ZombieBase botNetworks = SimplePool.Spawn<ZombieBase>(poolType, wayPoint.WayPoints[0].position, Quaternion.Euler(0, 180, 0));
            botNetworks.OnInit(wayPoint);
            botInScene.Add(botNetworks);

            yield return new WaitForSeconds(_botConfig.botDelaySpawn);
        }
    }

    public void RemoveBotDead(ZombieBase botNetworks)
    {
        _currentBot--;
        botInScene.Remove(botNetworks);
        float progress = (float) KillBot / (float)AllBotsToSpawn;
        EventManager.Invoke(EventName.UpdateGameProcess, progress);
    }
    
    private IEnumerator IEDelaySpawnGiftWeapon81(float _timer)
    {
        yield return new WaitForSeconds(_timer);
        giftWeapon81.SetActive(true);
    }
}
