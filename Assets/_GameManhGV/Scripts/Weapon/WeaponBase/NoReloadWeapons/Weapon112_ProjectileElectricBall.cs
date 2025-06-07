using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public class Weapon112_ProjectileElectricBall : ProjectileExplosionBase
{
    [SerializeField] private ParticleSystem _electricBallEffectChid;
    [SerializeField] private Transform _electricBallTransformChid;
    
    [Header("Max Size and Damage")]
    [SerializeField] int _damageElectricLine = 10;
    [SerializeField] float _maxSize = 1f;
    [SerializeField] int _maxDamage = 200;
    [SerializeField] float _size;

    Dictionary<GameObject, bool> _damageDictionary = new Dictionary<GameObject, bool>();
    private List<Effect> _effectElectricLines = new List<Effect>();

    public override void OnInit(Vector3 _direction, int _percentSize)
    {
        base.OnInit(_direction, _percentSize);
        _electricBallTransformChid.localScale =Vector3.one*(_maxSize* _percentSize / 100f);
        _damage = (int)(_maxDamage * _percentSize / 100f);
        _electricBallEffectChid.Play();
        _damageDictionary.Clear();
        _effectElectricLines.Clear();
    }
    
    protected override void Update()
    {
        base.Update();
        CheckDienGiat();
    }

    private float colsLeng = 0;
    public void CheckDienGiat()
    {
        Collider[] cols = Physics.OverlapSphere(TF.position, _radiusExplosion, _layerHit);
        if (cols.Length == colsLeng)
            return;
        else
            colsLeng = cols.Length;
        
        List<Transform> lstRoot = new List<Transform> ();
        foreach (Collider col in cols)
        {
            if (!lstRoot.Contains(col.gameObject.transform.root))
            {
                lstRoot.Add(col.gameObject.transform.root);
            }
        }
        print(lstRoot.Count);
        foreach(var elem in lstRoot)
        {
            ITakeDamage iTakeDamage = elem.gameObject.GetComponentInParent<ITakeDamage>();
            if (iTakeDamage!=null&&_damageDictionary.ContainsKey(iTakeDamage.GetTransform().gameObject))
            {
                print("Cos key");
                continue;
            }
            if (iTakeDamage != null)
            {
                var damageInfo = new DamageInfo()
                {
                    damageType = DamageType.Normal,
                    damage = _damageElectricLine,
                    name = elem.gameObject.name,
                };
                _damageDictionary.Add(iTakeDamage.GetTransform().gameObject, true);
                CreateElectricLine(iTakeDamage.GetTransform());
                iTakeDamage.TakeDamage(damageInfo);
            }
        }
    }

    public override void OnDespawn()
    {
        base.OnDespawn();
        colsLeng = 0;
        foreach (Effect effect in _effectElectricLines)
        {
            effect.OnDespawn();
        }
        _damageDictionary.Clear();
        _effectElectricLines.Clear();
    }

    public void CreateElectricLine(Transform _pointTarget)
    {
        Effect effectElectric = SimplePool.Spawn<Effect>(GameConstants.PoolType.vfx_electricLine, TF.position, Quaternion.identity);
        effectElectric.OnInit(TF, _pointTarget);
        _effectElectricLines.Add(effectElectric);
    }
}
