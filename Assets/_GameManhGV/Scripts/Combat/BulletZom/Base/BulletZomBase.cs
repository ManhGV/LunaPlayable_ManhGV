using UnityEngine;
using Image = UnityEngine.UI.Image;

public class BulletZomBase : GameUnit, ITakeDamage
{
#if UNITY_EDITOR
    public GameConstants.ProjecctileZombie projecctileTyle;
    private void OnValidate()
    {
        PoolType =(GameConstants.PoolType) projecctileTyle;
    }
#endif
    [SerializeField] protected bool important; // true: bất tử
    [SerializeField] protected int _dame;
    [SerializeField] protected BoxCollider colliderThis;
    [Header("Health")]
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;
    [SerializeField] protected Image _healthFill;
    protected bool _isDead;

    protected Vector3 posPlayer;

    public virtual void OnInit(Vector3 _posPlayer)
    {
        currentHealth = maxHealth;
        _isDead = false;
        posPlayer = _posPlayer;
    }

    protected virtual void OnTakeDamagePlayer()
    {
        print("TODO: TakeDamage");
        EventManager.Invoke(EventName.OnTakeDamagePlayer, _dame);
        _isDead = true;
    }

    public virtual void OnDespawn()
    {
        SimplePool.Despawn(this);
    }

    public virtual void OnDead()
    {
        
    }
    
    public virtual void TakeDamage(DamageInfo damageInfo)
    {
        if (_isDead)
            return;

        currentHealth -= damageInfo.damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            _isDead = true;
        }
        _healthFill.fillAmount = (float)currentHealth / (float)maxHealth;
    }

    public Transform GetTransformThis() => TF;
    public Transform GetTransformCenter() => TF;
}