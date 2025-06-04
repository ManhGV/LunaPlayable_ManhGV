using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class RocketController : Singleton<RocketController>
{
    [SerializeField] private Transform _posSpawn;
    [SerializeField] private Animation _fireAnim;
    public int currentAmount;

    [Header("Man Hinh + tutorial")]
    [SerializeField] private Transform _Portrait;
    [SerializeField] private Transform _Landscape;
    [SerializeField] private GameObject _body;
    [SerializeField] private float _speedMoveTurorial;

    [Header("Snake Camera")] 
    [SerializeField] Transform shakeCam; // Biến để tham chiếu đến MainCamera

    [SerializeField] private float shakeCamMin;
    [SerializeField] private float shakeCamMax;
    [SerializeField] private float duration = 0.3f; // Thời gian rung lắc
    [SerializeField] private float magnitude;

    [Header("ExplosionAudio")] 
    public AudioSource _audioSource;
    public AudioClip explosionAudio;

    [Header("Tutorial")] 
    public bool instructRoket = false;
    
    [Header("Cooldown")]
    [SerializeField] private float cooldownTime = 2.5f; // Thời gian chờ giữa các lần bắn
    [HideInInspector] public float timer;
    protected override void Awake()
    {
        base.Awake();
        _body.SetActive(false);
        _fireAnim.Play("FPS_anim_W38_Idle");
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Fire();
        }

        if (Input.GetKeyDown(KeyCode.A))
            InstructRocket();
        if (Input.GetKeyDown(KeyCode.R))
        {
            SetMoveTutorial();
        }
    }

    public void PlayAudioExplosion()
    {
        _audioSource.PlayOneShot(explosionAudio);
    }

    public void Fire()
    {
        if(timer > 0)
            return;
        
        timer = cooldownTime;
        if (currentAmount > 0)
        {
            _fireAnim.Play("FPS_anim_W38_Fire");
            Rocket rocket = SimplePool.Spawn<Rocket>(GameConstants.PoolType.bullet_Rocket, _posSpawn.position, Quaternion.identity);
            rocket.OnInit(WeaponBase.Instance.GizmodTuVe());
            currentAmount--;
            StartCoroutine(IEReload());
        }
        else
        {
            Debug.LogWarning("Out of bullet");
        }
    }

    public float GetFillAmount()
    {
        return timer/cooldownTime;
    }
    
    public void SnakeCameraRocket() => StartCoroutine(ShakeCamera(duration, magnitude));

    // Thêm hàm rung lắc camera
    private IEnumerator IEReload()
    {
        yield return new WaitForSeconds(cooldownTime - 1f);
        _fireAnim.Play("FPS_anim_W38_Reload");
        yield return new WaitForSeconds(1f);
        _fireAnim.Play("FPS_anim_W38_Idle");
    }
    private IEnumerator ShakeCamera(float duration, float magnitude)
    {
        Quaternion originalRot = shakeCam.localRotation;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(shakeCamMin, shakeCamMax) * magnitude;
            float y = Random.Range(shakeCamMin, shakeCamMax) * magnitude;

            shakeCam.localRotation = originalRot * Quaternion.Euler(x, y, 0);

            elapsed += Time.deltaTime;

            yield return null;
        }

        shakeCam.localRotation = originalRot;
        EventManager.Invoke(EventName.OnCheckShakeCam, shakeCam.localEulerAngles);
    }

    public void InstructRocket()
    {
        //print("Hướng dẫn rocket");
        if (!instructRoket)
        {
            instructRoket = true;
            EventManager.Invoke(EventName.InstructRocket, true);
        }
    }

    public void SetMoveTutorial()
    {
        transform.localPosition = new Vector3(0, 0, -1);
        if (Screen.width < Screen.height)
            StartCoroutine(IEMoveToPoint());
        else
            StartCoroutine(IEMoveToPoint());
        
        StartCoroutine(IEEnbleTutorial());
    }

    public IEnumerator IEEnbleTutorial()
    {
        bool _break = true;
        while (_break)
        {
            if (Vector3.Distance(Vector3.zero, TF.position) < .1f)
            {
                InstructRocket();
                _break = false;
            }

            yield return null;
        }
    }
    public IEnumerator IEMoveToPoint()
    {
        _body.SetActive(true);
        while (Vector3.Distance(Vector3.zero, TF.localPosition) > .1f)
        {
            TF.localPosition = Vector3.MoveTowards(TF.localPosition, Vector3.zero, Time.deltaTime * _speedMoveTurorial);
            yield return null;
        }
        TF.localPosition = Vector3.zero;
        InstructRocket();
    }
}