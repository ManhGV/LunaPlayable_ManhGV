using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DetectorBase : MonoBehaviour,ITakeDamage
{
    [Header("Look Camera")]
    [SerializeField] private Transform cameraTransform;
    
    [Header("Phóng to sau đó thu nhỏ Derector")]
    [SerializeField] private float _originalScaleDefault = 1f;
    [SerializeField] private float _scaleSpeed = 60f;  
    [SerializeField] private float _scaleMultiplier = 10f;
    [FormerlySerializedAs("_detectorUI")] [SerializeField] private Transform _detectorLookPlayer;
    private Coroutine _currentCoroutine;

    [Header("Health")] 
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private int _currentHealth;
    public Image healthImage;
    bool _deadDetector;

    private void OnEnable()
    {
        OnInit();
    }

    public void OnInit()
    {
        gameObject.SetActive(true);
        healthImage.fillAmount = 1f;
        _deadDetector = false;
        _currentHealth = _maxHealth;
        _detectorLookPlayer.localScale =Vector3.one * _originalScaleDefault;
        Play();
    }

    private void Awake()
    {
        cameraTransform = GameManager.Instance.GetMainCameraTransform();
    }

    private void LateUpdate()
    {
        if (cameraTransform != null)
            _detectorLookPlayer.LookAt(cameraTransform);
    }
    
    public void Play()
    {
        if (_currentCoroutine != null)
            StopCoroutine(_currentCoroutine);

        _currentCoroutine = StartCoroutine(AnimateScale());
    }

    private IEnumerator AnimateScale()
    {
        Vector3 targetScale = Vector3.one * _originalScaleDefault;
        Vector3 startScale = targetScale * _scaleMultiplier;

        // Gán giá trị phóng to ngay lập tức
        _detectorLookPlayer.localScale = startScale;

        while (true)
        {
            if (Vector3.Distance(_detectorLookPlayer.localScale, targetScale) > 0.001f)
                _detectorLookPlayer.localScale = Vector3.MoveTowards(_detectorLookPlayer.localScale, targetScale, _scaleSpeed * Time.deltaTime);
            else
                break;
            yield return null;
        }
        _currentCoroutine = null;
    }

    public void TakeDamage(DamageInfo damageInfo)
    {   
        if(_deadDetector)
            return;
        _currentHealth -= damageInfo.damage;
        
        if (_currentHealth <= 0)
        {
            _deadDetector = true;
            _currentHealth = 0;
            OnDead();
        }
        healthImage.fillAmount = (float)_currentHealth / (float)_maxHealth;
    }

    public Transform GetTransform() => this.transform;

    public virtual void OnDead()
    {
        if (_currentCoroutine != null)
            StopCoroutine(_currentCoroutine);
        gameObject.SetActive(false);
    }
}
