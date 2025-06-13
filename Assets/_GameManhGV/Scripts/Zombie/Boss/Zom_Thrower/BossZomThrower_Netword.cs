using System.Collections;
using UnityEngine;

public class BossZomThrower_Netword : BossNetwork
{
    [Header("Jump Settings")]
    public float jumpMaxHeightByDistance = 0.5f;
    public float jumpMaxHeightAppend = 1f;
    public float jumpSpeedXZ = 5f;
    public ParticleSystem vfxDust; // VFX bụi khi chạm đất
    private bool isJumping = false;

    public bool JumpToTarget(Vector3 targetPos)
    {
        // Bắt đầu coroutine nhảy
        isJumping = true;
        if(Vector3.Distance(TF.position, targetPos) < 0.5f)
        {
            Debug.LogWarning("Quá gần không thể nhảy");
            isJumping = false; // Nếu vị trí đích quá gần, không cần nhảy
            return false;
        }
        else
        {
            StartCoroutine(JumpCoroutine(targetPos));
            return true;
        }
    }
    
    private IEnumerator JumpCoroutine(Vector3 endJumpPos)
    {
        Vector3 startJumpPos = TF.position;
        Vector3 flatStart = new Vector3(startJumpPos.x, 0f, startJumpPos.z);
        Vector3 flatEnd = new Vector3(endJumpPos.x, 0f, endJumpPos.z);
        float distance = Vector3.Distance(flatStart, flatEnd);

        float jumpHeight = Mathf.Max(jumpMaxHeightByDistance * distance, 1.5f) + jumpMaxHeightAppend;
        float jumpDuration = distance / jumpSpeedXZ;
        float timeActiveAnimator = jumpDuration - .1f; // Thời gian kích hoạt lại animator
        float currentTime = 0f;

        // Xoay hướng ban đầu
        yield return RotateToDirection(endJumpPos - startJumpPos);

        // Play animation và delay trước khi bắt đầu di chuyển
        ChangeAnimAndType("Jump",0);
        yield return new WaitForSeconds(.16f); // Thời gian delay trước khi bắt đầu di chuyển
        ChangeAnimAndType("Jump",1);

        Vector3 lastFramePos = startJumpPos;
        while (currentTime < jumpDuration)
        {
            float t = currentTime / jumpDuration;
            // Di chuyển tuyến tính theo XZ
            Vector3 pos = Vector3.Lerp(startJumpPos, endJumpPos, t);

            // Tính độ cao theo parabol
            float height = 4 * jumpHeight * t * (1 - t);
            pos.y += height;

            TF.position = pos;

            // Xoay chỉ theo trục Y (hướng ngang)
            Vector3 direction = pos - lastFramePos;
            direction.y = 0;
            if (direction.sqrMagnitude > 0.001f)
                TF.rotation = Quaternion.LookRotation(direction);

            lastFramePos = TF.position;
            currentTime += Time.deltaTime;
            if (currentTime >= timeActiveAnimator) //34%
            {
                ChangeAnimAndType("Jump",2);
            }
            yield return null;
        }

        TF.position = endJumpPos;

        if (vfxDust != null)
            vfxDust.Play();

        yield return new WaitForSeconds(.1f);
        isJumping = false;
    }

    private IEnumerator RotateToDirection(Vector3 direction)
    {
        direction.y = 0;
        Quaternion targetRot = Quaternion.LookRotation(direction.normalized);
        while (Quaternion.Angle(TF.rotation, targetRot) > 1f)
        {
            TF.rotation = Quaternion.Slerp(TF.rotation, targetRot, 0.15f);
            yield return null;
        }
    }
    public bool IsJumping() => isJumping;
}