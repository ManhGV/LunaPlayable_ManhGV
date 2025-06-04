using System;
using UnityEngine;

public class RewardNetwork : MonoBehaviour, IReward
{
    [Header("Audio")]
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip[] _audioClip;
    [SerializeField] float _audioVolume;
    [Header("Health")]
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
        PlayCollectSound(_audioVolume);
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
    
    public void PlayCollectSound(float _volume)
    {
        _audioSource.volume = _volume;
        _audioSource.PlayOneShot(_audioClip[UnityEngine.Random.Range(0, _audioClip.Length)]);
    }
}
