using System.Collections;
using UnityEngine;

public class EventAnim_BossZomSwat : MonoBehaviour
{
    [SerializeField] private ExplosionDor explosionDoor;
    
    [Header("Snake Camera")]
    [SerializeField] private Transform _cameraSnakeCutScene;
    [SerializeField] private float shakeCamMin;
    [SerializeField] private float shakeCamMax;
    private Coroutine shakeCoroutine;
    
    public void ExplosionDoor()
    {
        if (explosionDoor != null)
        {
            explosionDoor.ExplosionDoor();
        }
    }
    
    public void ShakeCamera()
    {
        StartCoroutine(ShakeCamera(0.5f, 0.1f));
    }
    
    /// <summary>
    /// Rung camera liên tục cho tới khi StopShakeCamera() được gọi
    /// </summary>
    public void StartShakeCameraLoop(float magnitude)
    {
        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(ShakeCameraLoop(magnitude));
    }

    /// <summary>
    /// Dừng rung camera
    /// </summary>
    public void StopShakeCamera()
    {
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
