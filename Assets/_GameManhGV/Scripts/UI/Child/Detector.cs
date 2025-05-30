using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Detector : MonoBehaviour
{
    [SerializeField] private bool PlayAwake;
    private float _scaleDefault = 1f;
    [Header("Look Camera")]
    [SerializeField] private Transform cameraTransform;
    
    [Header("Phóng to sau đó thu nhỏ Derector")]
    [SerializeField] private float _scaleSpeed = 60f;  
    [SerializeField] private float _scaleMultiplier = 10f;

    private Vector3 _originalScale;
    private bool _isPlaying;
    
    [SerializeField] private float lifeTime = 2f;
    private Coroutine _currentCoroutine;
    private Coroutine _ActiveFalseCoroutine;
    [SerializeField] private Transform _detector;

    [Header("Health")] 
    [SerializeField] private bool _isBoss;
    [SerializeField] private BossZomSwat_Attack _bossZomSwatAttack;
    [SerializeField] private int _skillIndex;
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private int _currentHealth;
    public Image healthImage;
    bool _deadDetector;
    

    private void Awake()
    {
        cameraTransform = GameManager.Instance.GetMainCameraTransform();
    }

    private void OnEnable()
    {
        healthImage.fillAmount = 1f;
        _deadDetector = false;
        _currentHealth = _maxHealth;
        _detector.localScale = Vector3.one * _scaleDefault;
        _originalScale = Vector3.one * _scaleDefault;
        _ActiveFalseCoroutine = StartCoroutine(DisableThis());
        
        if(PlayAwake)
            Play();
        else
            gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (_ActiveFalseCoroutine != null)
            StopCoroutine(_ActiveFalseCoroutine);
    }

    private void LateUpdate()
    {
        if (cameraTransform != null)
        {
            _detector.LookAt(cameraTransform);
        }
    }

    private IEnumerator DisableThis()
    {
        yield return new WaitForSeconds(lifeTime);
        gameObject.SetActive(false);
    }
    
    public void Play()
    {
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
        }

        _currentCoroutine = StartCoroutine(AnimateScale());
    }

    private IEnumerator AnimateScale()
    {
        Vector3 targetScale = _originalScale;
        Vector3 startScale = _originalScale * _scaleMultiplier;

        // Gán giá trị phóng to ngay lập tức
        _detector.localScale = startScale;

        while (true)
        {
            if (Vector3.Distance(_detector.localScale, targetScale) > 0.001f)
                _detector.localScale = Vector3.MoveTowards(_detector.localScale, targetScale, _scaleSpeed * Time.deltaTime);
            else
                break;
            yield return null;
        }
        _currentCoroutine = null;
    }
    
    public void SetHealthImage(int damage)
    {
        if(_deadDetector)
            return;
        _currentHealth -= damage;
        
        if (_currentHealth <= 0)
        {
            _deadDetector = true;
            _currentHealth = 0;
            healthImage.fillAmount = 0;
            _bossZomSwatAttack.Stun();
        }
        healthImage.fillAmount = (float)_currentHealth / (float)_maxHealth;
    }
}
