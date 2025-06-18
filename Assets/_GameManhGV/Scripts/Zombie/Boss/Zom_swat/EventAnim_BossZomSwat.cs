using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class EventAnim_BossZomSwat : MonoBehaviour
{
    [SerializeField] private BossZomSwatNetword _bossZomSwat;
    [SerializeField] private ExplosionDor _explosionDoor;
    [SerializeField] private BotConfigSO _botConfig;
    
    [Header("Canvas - Active swat")]
    [SerializeField] GameObject _canvasSwat;
    
    [SerializeField] private Transform newMaincamera;
    [Header("Snake Camera")]
    [SerializeField] private Transform _cameraSnakeCutScene;
    [SerializeField] private float shakeCamMin;
    [SerializeField] private float shakeCamMax;
    private Coroutine shakeCoroutine;
    
    public void TakeDamagePlayer()
    {
        EventManager.Invoke(EventName.OnTakeDamagePlayer, _botConfig.damage);
    }
    
    public void ExplosionDoor()
    {
        _explosionDoor.gameObject.SetActive(true);
        if (_explosionDoor != null)
        {
            _explosionDoor.ExplosionDoor();
            //CutSceneCam.Instance.MoveFromAToB(1,2,.65f,70f);
        }
    }
    
    public void ChangeCam()=> _cameraSnakeCutScene = newMaincamera;
    
    public void ShakeCamera()
    {
        StartCoroutine(ShakeCamera(0.5f, 0.1f));
    }
    
    /// <summary>
    /// Rung camera liên tục cho tới khi StopShakeCamera() được gọi
    /// </summary>
    public void StartShakeCameraLoop(float magnitude)
    {
        _canvasSwat.SetActive(true);
        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(ShakeCameraLoop(magnitude));
    }

    public void PlaySoundIndex(int _index)
    {
        _bossZomSwat.PlayAudioVoice(_index,1, false);
    }

    /// <summary>
    /// Dừng rung camera
    /// </summary>
    public void StopShakeCamera()
    {
        _canvasSwat.SetActive(false);
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            shakeCoroutine = null;
        }
    }

    private IEnumerator ShakeCameraLoop(float magnitude)
    {
        Quaternion originalRot = _cameraSnakeCutScene.localRotation;

        while (true)
        {
            float x = Random.Range(shakeCamMin, shakeCamMax) * magnitude;
            float y = Random.Range(shakeCamMin, shakeCamMax) * magnitude;

            _cameraSnakeCutScene.localRotation = originalRot * Quaternion.Euler(x, y, 0);

            yield return null;
        }
    }
    
    protected IEnumerator ShakeCamera(float duration, float magnitude)
    {
        Quaternion originalRot = _cameraSnakeCutScene.localRotation;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(shakeCamMin, shakeCamMax) * magnitude;
            float y = Random.Range(shakeCamMin, shakeCamMax) * magnitude;

            _cameraSnakeCutScene.localRotation = originalRot * Quaternion.Euler(x, y, 0);

            elapsed += Time.deltaTime;

            yield return null;
        }

        _cameraSnakeCutScene.localRotation = originalRot;
    }
}
