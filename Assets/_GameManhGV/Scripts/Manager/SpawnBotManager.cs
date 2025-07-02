using static GameConstants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] GameObject giftPowerUp;
    
    [Header("BossZom")]
    [SerializeField] GameObject _boss;
    [SerializeField] bool canCallBoss;
    
    protected override void Awake()
    {
        base.Awake();
        foreach (BotConfig VARIABLE in dataBotSpawn.fightRound.botConfigs)
            foreach (SpawmQuantity quantity in VARIABLE.spawmQuantities)
                AllBotsToSpawn += quantity.botQuantity;
        _currentBot = AllBotsToSpawn;
    }

    private void Start()
    {
        float progress = (float) KillBot / (float)AllBotsToSpawn;
        EventManager.Invoke(EventName.UpdateGameProcess, progress);
    }

    public IEnumerator IECallBossZom(float time)
    {
        if (!GameManager.Instance.endGame)
        { 
            yield return new WaitForSeconds(time);
            PlayerHP.Instance.ClearListDamage();
            yield return new WaitForSeconds(1.5f);
            GameManager.Instance.ActiveSoundCombat();
            _boss.SetActive(true);
        }
    }

    public void CallAllBotAttack()
    {
        foreach (BotZombieNor_Start VARIABLE in botHaveStart)
            VARIABLE.CallOnTakeDamage();
    }
    
    public void SpawnBot()
    {
        StartCoroutine(IECallBossZom(20f));
        StartCoroutine(IEDelaySpawnGiftWeapon81());
        foreach(BotConfig botConfig in dataBotSpawn.fightRound.botConfigs)
            StartCoroutine(IEOnSpawnBot(botConfig));
    }

    IEnumerator IEOnSpawnBot(BotConfig _botConfig)
    {
        List<SpawmQuantity> spawmQuantities = _botConfig.spawmQuantities;
        
        foreach (SpawmQuantity VARIABLE in spawmQuantities)
        {
            yield return new WaitForSeconds(VARIABLE.WaitToSpawn);
            GameConstants.PoolType poolType;
            WayPoint wayPoint;

            for (int i = 0; i < VARIABLE.botQuantity; i++)
            {
                if(GameManager.Instance.endGame)
                    yield break;
                poolType = _botConfig.botType;
                wayPoint = _pathManager.GetWayPoint(VARIABLE.PoinSpawnbot);
                ZombieBase botNetworks = SimplePool.Spawn<ZombieBase>(poolType, wayPoint.WayPoints[0].position, Quaternion.Euler(0, 180, 0));
                botNetworks.OnInit(wayPoint);
                botInScene.Add(botNetworks);

                yield return new WaitForSeconds(VARIABLE.botDelaySpawn);
            }
        }
    }

    public void RemoveBotDead(ZombieBase botNetworks)
    {
        _currentBot--;
        botInScene.Remove(botNetworks);
        float progress = (float) KillBot / (float)AllBotsToSpawn;
        EventManager.Invoke(EventName.UpdateGameProcess, progress);
    }
    
    private IEnumerator IEDelaySpawnGiftWeapon81()
    {
        yield return new WaitForSeconds(4f);
        giftPowerUp.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        giftWeapon81.SetActive(true);
    }
    public void DespawnAllBot(float _timer)=>StartCoroutine(IEDespawnAllBot(_timer));
    
    private IEnumerator IEDespawnAllBot(float _timer)
    {
        yield return new WaitForSeconds(_timer);
        for (int i = 0; i < botInScene.Count; i++)
            botInScene[i].OnDespawn();
    }
}
