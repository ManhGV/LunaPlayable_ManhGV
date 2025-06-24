using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossZomRevenant_Netword : BossNetwork
{
    [Header("Jump Settings")]
    public float jumpMaxHeightByDistance = 0.5f;
    public float jumpMaxHeightAppend = 1f;
    public float jumpSpeedXZ = 5f;
    public ParticleSystem vfxDust; // VFX bụi khi chạm đất
    private bool isJumping = false;

    [Header("Check Chạm tường ngừng đi")] 
    [SerializeField] private LayerMask _layerCantMove;

    public bool CheckForardChamVaoGround()
    {
        Vector3 origin = transform.position + Vector3.up * 1.5f;
        Vector3 forward = transform.forward;

        // Debug rays
        // Debug.DrawRay(origin, forward * 0.8f, Color.red, 1f);
        // Debug.DrawRay(origin, Quaternion.Euler(0, -45, 0) * forward * .9f, Color.yellow, 1f);
        // Debug.DrawRay(origin, Quaternion.Euler(0, 45, 0) * forward * .9f, Color.yellow, 1f);

        // Check collisions
        if (Physics.Raycast(origin, forward, 0.8f, _layerCantMove)) return true;
        if (Physics.Raycast(origin, Quaternion.Euler(0, -45, 0) * forward, .9f, _layerCantMove)) return true;
        if (Physics.Raycast(origin, Quaternion.Euler(0, 45, 0) * forward, .9f, _layerCantMove)) return true;

        return false;
    }

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
    
    private IEnumerator JumpCoroutine(Vector3 endJumpPos)
    {
        Vector3 startJumpPos = TF.position;
        Vector3 flatStart = new Vector3(startJumpPos.x, 0f, startJumpPos.z);
        Vector3 flatEnd = new Vector3(endJumpPos.x, 0f, endJumpPos.z);
        float distance = Vector3.Distance(flatStart, flatEnd);

        float jumpHeight = Mathf.Max(jumpMaxHeightByDistance * distance, 1.5f) + jumpMaxHeightAppend;
        float jumpDuration = distance / jumpSpeedXZ;
        float timeActiveAnimator = jumpDuration - .78f; // Thời gian kích hoạt lại animator
        float currentTime = 0f;

        // Xoay hướng ban đầu
        yield return RotateToDirection(endJumpPos - startJumpPos);

        // Play animation và delay trước khi bắt đầu di chuyển
        //animator.CrossFade(jumpAnimHash, 0, 0, 0);
        animator.Play("Jump");
        yield return new WaitForSeconds(1.2f); // Thời gian delay trước khi bắt đầu di chuyển
        PlayAudioVoice(0,1,false);
        animator.enabled = false;

        Vector3 lastFramePos = startJumpPos;
        while (currentTime < jumpDuration)
        {
            float t = currentTime / jumpDuration;
            // Di chuyển tuyến tính theo XZ
            Vector3 pos = Vector3.Lerp(startJumpPos, endJumpPos, t);

            // Tính độ cao theo parabol
            float height = 4 * jumpHeight * t * (1 - t); // hoặc: jumpYCurve.Evaluate(t) * jumpHeight;
            pos.y += height;

            TF.position = pos;

            // Xoay chỉ theo trục Y (hướng ngang)
            Vector3 direction = pos - lastFramePos;
            direction.y = 0;
            if (direction.sqrMagnitude > 0.001f)
                TF.rotation = Quaternion.LookRotation(direction);

            lastFramePos = TF.position;
            currentTime += Time.deltaTime;
            if (currentTime >= timeActiveAnimator && !animator.enabled) //34%
            {
                animator.enabled = true; // Bật lại animator nếu thời gian nhảy ngắn
                animator.Play("Jump", 0, 1.12f / 3.27f); // chạy từ 1.28f jump \ 3.27f là tổng thời gian của anim jump
            }
            yield return null;
        }

        TF.position = endJumpPos;

        if (vfxDust != null)
            vfxDust.Play();
        PlayAudioVoice(1, 1, false);
        yield return new WaitForSeconds(1.85f);
        isJumping = false;
        isImmortal = false;
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
    
    public Vector3 GetRandomPointBehindZombie(float _radius)
    {
        // Lấy hướng ngược lại của forward (hướng đằng sau zombie)
        Vector3 backward = -TF.forward.normalized;

        // Random một góc từ -90 đến +90 độ (tức trong nửa vòng tròn đằng sau)
        float angle = Random.Range(-90f, 90f) * Mathf.Deg2Rad;

        // Tính hướng xoay dựa trên góc và hướng backward
        Vector3 direction = Quaternion.Euler(0, angle * Mathf.Rad2Deg, 0) * backward;

        // Nhân với bán kính
        Vector3 offset = direction.normalized * _radius;

        // Trả về vị trí mới
        return TF.position + offset;
    }

    public override void SetActiveDetectors(bool _active, int _skillType)
    {
        base.SetActiveDetectors(_active, _skillType);
        detectors[_skillType].SetActive(_active);
    }
}