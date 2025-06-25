using System.Collections;
using System.Collections.Generic;
using static GameConstants;
using UnityEngine;
using UnityEngine.Serialization;

public class Bot_zom_deadExplosion_Dead : StateBase<ZomAllState,BotNetwork>
{
    [SerializeField] private GameObject _body;
    [SerializeField] private ParticleSystem _vfxExplosion;
    
    [Header("Explosion")]
    public float _radiusExplosion;
    [SerializeField] private LayerMask _layerHit;
    
    [SerializeField] private float waitExplosion;
    [SerializeField] private float timerDespawn;
    private float timer;
    public void OnInit()
    {
        _body.SetActive(true);
    }
    
    public override void EnterState()
    {
        StartCoroutine(IEExplosion());
    }

    public override void UpdateState()
    {
        
    }

    public override void ExitState()
    {
        
    }
    
    IEnumerator IEExplosion()
    {
        thisBotNetworks.ChangeAnim("Dead");
        yield return new WaitForSeconds(waitExplosion);
        _body.SetActive(false);
        thisBotNetworks.PlayAudioVoice(0,1,false);
        if (Vector3.Distance(thisBotNetworks.GetTransformCenter().position, thisBotNetworks.GetWayPointEndMove()) < _radiusExplosion)
            EventManager.Invoke(EventName.OnTakeDamagePlayer,thisBotNetworks.BotConfigSO.damage);
        _vfxExplosion.Play();
        CheckHitExplosion();
        yield return new WaitForSeconds(timerDespawn);
        thisBotNetworks.OnDespawn();
    }
    
    public void CheckHitExplosion()
    {
        Collider[] cols = Physics.OverlapSphere(thisBotNetworks.GetTransformCenter().position, _radiusExplosion, _layerHit);
        if(cols.Length <= 0)
            return;
        
        List<Transform> lstRoot = new List<Transform> ();
        foreach (Collider col in cols)
        {
            if (!lstRoot.Contains(col.gameObject.transform.root))
            {
                lstRoot.Add(col.gameObject.transform.root);
            }
        }
        foreach(var elem in lstRoot)
        {
            var takeDamageController = elem.gameObject.GetComponentInParent<ITakeDamage>();
            
            if(takeDamageController != null)
            {
                var damageInfo = new DamageInfo()
                {
                    damageType = DamageType.Gas,
                    damage = 101,
                    name = elem.gameObject.name,
                };
                takeDamageController.TakeDamage(damageInfo);
            }
        }
    }
}