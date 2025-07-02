using System;
using UnityEngine;

public class BotNetwork : ZombieBase
{
    [Header("Dead Explosion")]
    public bool IsDeadExplosion;
    public Vector3 _posDamageGas;
    
    public Action<bool> ZombieDeadExplosion { get; set; }

    public override void OnInit(WayPoint _wayPoint)
    {
        base.OnInit(_wayPoint);
        IsDeadExplosion = false;
    }

    public override void TakeDamage(DamageInfo damageInfo)
    {
        if(IsDeadExplosion || isDead || isImmortal)
            return;
        
        OnTakeDamage?.Invoke(damageInfo.damage);
        
        if(healthBarTransform != null && damageInfo.damageType != DamageType.Gas)//||healthBarTransform != null && isBoss && damageInfo.damageType == DamageType.Gas)
        {
            CacularHealth(damageInfo);
            healthBarTransform.gameObject.SetActive(true);
            
            if (hideHealthBarCoroutine != null)
                StopCoroutine(hideHealthBarCoroutine);

            hideHealthBarCoroutine = StartCoroutine(IEHideHealthBarAfterDelay());
        }
        else if(damageInfo.damageType == DamageType.Gas)
        {
            ZombieDeadExplosion?.Invoke(true);
            _currentHealth = 0;
            IsDeadExplosion = true;
            CacularHealth(damageInfo);
        }
    }

    /// <summary>
    /// Get hướng nổ bay  0 = trước;  1 = phải;  2 = trái;  3 = sau;
    /// </summary>
    /// <returns></returns>
    public int GetNearestDirection()
    {
        Vector3 toTarget = (_posDamageGas - TF.position).normalized;
    
        // Các hướng cơ bản trong local space
        Vector3 forward = TF.forward;
        Vector3 right = TF.right;
        Vector3 left = -TF.right;
        Vector3 back = -TF.forward;
        // 0 = trước
        // 1 = phải
        // 2 = trái
        // 3 = sau
        // Tính độ tương đồng (dot product)
        float dotForward = Vector3.Dot(toTarget, forward);
        float dotRight = Vector3.Dot(toTarget, right);
        float dotLeft = Vector3.Dot(toTarget, left);
        float dotBack = Vector3.Dot(toTarget, back);
    
        // Tìm hướng có dot lớn nhất (góc gần nhất)
        float maxDot = dotForward;
        int direction = 0; // 0 = trước
    
        if (dotRight > maxDot)
        {
            maxDot = dotRight;
            direction = 1;
        }
    
        if (dotLeft > maxDot)
        {
            maxDot = dotLeft;
            direction = 2;
        }
    
        if (dotBack > maxDot)
        {
            maxDot = dotBack;
            direction = 3;
        }
    
        return direction;
    }

    public override void BotDead()
    {
        base.BotDead();
        AchievementEvaluator.Instance.OnBotKilled(1.8f,false);
    }

    public Vector3 GetWayPointEndMove() => wayPoint.WayPoints[wayPoint.WayPoints.Count - 1].position;
}