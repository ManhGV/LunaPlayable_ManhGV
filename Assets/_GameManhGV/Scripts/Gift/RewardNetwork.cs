using System;
using UnityEngine;

public class RewardNetwork : MonoBehaviour, IReward
{
    [SerializeField] int _maxHealth = 100;
    [SerializeField] int _currentHealth;
    [SerializeField] private bool isCollected;
    public Action<int> OnTakeDamage { get; set; }
    public Action OnRewardCollected { get; set; }

    private void Awake()
    {
        OnRewardCollected += Collect;
        OnTakeDamage += CalculateHealth;
        _currentHealth = _maxHealth;
    }

    public void TakeCollect(int damage)
    {
        if (isCollected) return;
        OnTakeDamage.Invoke(damage);
    }

    public void CalculateHealth(int damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            isCollected = true;
            OnRewardCollected.Invoke();
        }
    }

    public void Collect()
    {
        isCollected = true;
    }
}
