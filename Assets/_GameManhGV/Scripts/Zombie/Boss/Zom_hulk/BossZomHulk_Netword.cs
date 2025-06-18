using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossZomHulk_Netword : BossNetwork
{
    [Header("Jump Default Settings")]
    public float jumpMaxHeightByDistance = 0.5f;
    public float jumpMaxHeightAppend = 1f;
    public float jumpSpeedXZ = 5f;
    
    [Header("Jump Punch Settings")]
    public float distanceReductionToPlayer = 2f;
    public float jumpMaxHeightByDistance_JPunch = 0.5f;
    public float jumpMaxHeightAppend_JPunch = 1f;
    public float jumpSpeedXZ_JPunch = 5f;
    
    [Header("Jump Settings")]
    public ParticleSystem vfxDust; // VFX bụi khi chạm đất
    private bool isJumping = false;

    public bool JumpToTarget(Vector3 targetPos)
    {
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
    
    public bool JumpPunchToTarget(Vector3 targetPos)
    {
        targetPos.y = 0;
        Vector3 direction = (targetPos - TF.position).normalized;
        
        float originalDistance = Vector3.Distance(TF.position, targetPos);
        
        float newDistance = Mathf.Max(0f, originalDistance - distanceReductionToPlayer);
        
        targetPos = TF.position + direction * newDistance;
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
            StartCoroutine(JumpPunchCoroutine(targetPos));
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
        bool canChangeEnd = true;
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
            if (currentTime >= timeActiveAnimator && canChangeEnd) //34%
            {
                canChangeEnd = false;
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
    
    private IEnumerator JumpPunchCoroutine(Vector3 endJumpPos)
    {
        Vector3 startJumpPos = TF.position;
        Vector3 flatStart = new Vector3(startJumpPos.x, 0f, startJumpPos.z);
        Vector3 flatEnd = new Vector3(endJumpPos.x, 0f, endJumpPos.z);
        float distance = Vector3.Distance(flatStart, flatEnd);

        float jumpHeight = Mathf.Max(jumpMaxHeightByDistance_JPunch * distance, 1.5f) + jumpMaxHeightAppend_JPunch;
        float jumpDuration = distance / jumpSpeedXZ_JPunch;
        float timeActiveAnimator = jumpDuration - 0.18f; // Thời gian kích hoạt lại animator
        float currentTime = 0f;

        // Xoay hướng ban đầu
        yield return RotateToDirection(endJumpPos - startJumpPos);

        // Play animation và delay trước khi bắt đầu di chuyển
        ChangeAnimAndType("JumpPunch",0);
        yield return new WaitForSeconds(0.2f); // Thời gian delay trước khi bắt đầu di chuyển
        ChangeAnimAndType("JumpPunch",1);

        Vector3 lastFramePos = startJumpPos;
        bool canChangeEnd = true;
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
            if (currentTime >= timeActiveAnimator&& canChangeEnd)
            {
                canChangeEnd = false;
                ChangeAnimAndType("JumpPunch",2);
            }
            yield return null;
        }

        TF.position = endJumpPos;

        if (vfxDust != null)
            vfxDust.Play();

        yield return new WaitForSeconds(2f);
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