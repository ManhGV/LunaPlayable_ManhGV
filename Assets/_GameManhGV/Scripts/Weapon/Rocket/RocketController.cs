using System.Collections;
using UnityEngine;

public class RocketController : Singleton<RocketController>
{
    [SerializeField] private Transform _posSpawn;
    [SerializeField] private Animation _fireAnim;
    [SerializeField] private int currentBullet;
    
    [Header("Snake Camera")]
    [SerializeField] Transform shakeCam; // Biến để tham chiếu đến MainCamera
    [SerializeField] private float shakeCamMin;
    [SerializeField] private float shakeCamMax;
    [SerializeField] private float duration = 0.3f; // Thời gian rung lắc
    [SerializeField] private float magnitude;

    [Header("ExplosionAudio")] 
    public AudioSource _audioSource;
    public AudioClip explosionAudio;
    
    private void Awake()
    {
        _fireAnim.Play("FPS_anim_W38_Idle");
    }
    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) {
            Fire();
        }
    }

    public void PlayAudioExplosion()
    {
        _audioSource.PlayOneShot(explosionAudio);
    }
    
    public void Fire()
    {
        if(currentBullet > 0)
        {
            _fireAnim.Play("FPS_anim_W38_Fire");
            Rocket rocket = SimplePool.Spawn<Rocket>(GameConstants.PoolType.bullet_Rocket, _posSpawn.position, Quaternion.identity);
            rocket.OnInit(WeaponController.Instance.GizmodTuVe());
            currentBullet--;
        }
        else
        {
            Debug.LogWarning("Out of bullet");
        }
    }
    
    public void SnakeCameraRocket()=> StartCoroutine(ShakeCamera(duration, magnitude));
    
    // Thêm hàm rung lắc camera
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
}
