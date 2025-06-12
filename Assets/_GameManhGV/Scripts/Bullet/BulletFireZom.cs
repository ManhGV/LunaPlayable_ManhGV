using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletFireZom : GameUnit, ITakeDamage
{
#if UNITY_EDITOR
    public GameConstants.ProjecctileZombie projecctileTyle;
    private void OnValidate()
    {
        PoolType =(GameConstants.PoolType) projecctileTyle;
    }
#endif
    [Header("Referent")]
    [SerializeField] protected int _dame;
    [SerializeField] protected float _speed;

    [Header("Health")]
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;
    [SerializeField] private Image _healthFill;
    private bool _isDead;

    protected Vector3 dir;
    Vector3 posPlayer;

    public void OnInit(Vector3 _posPlayer)
    {
        currentHealth = maxHealth;
        _isDead = false;
        posPlayer = _posPlayer;
        dir = (_posPlayer - TF.position).normalized;
    }

    private void Update()
    {
        if (_isDead)
            return;
        TF.position += dir * _speed * Time.deltaTime;

        if (Vector3.Distance(posPlayer, TF.position) < .5f)
            OnTakeDamagePlayer();
    }

    private void OnTakeDamagePlayer()
    {
        print("TODO: TakeDamage");
        //EventManager.Invoke(EventName.OnTakeDamagePlayer, _dame);
        _isDead = true;
        OnDespawn();
    }

    public void OnDespawn()
    {
        SimplePool.Despawn(this);
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        if (_isDead)
            return;

        currentHealth -= damageInfo.damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            _isDead = true;
            OnDespawn();
        }
        _healthFill.fillAmount = (float)currentHealth / (float)maxHealth;
    }

    public Transform GetTransform() => TF;
}