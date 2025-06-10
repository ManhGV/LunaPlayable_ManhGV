using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallisticArmor : MonoBehaviour
{
    [SerializeField] private Transform _bossParent;
    
    [Header("Cấu hình")]
    public float fallSpeed = 5f;            // Tốc độ rơi
    public float rotationSpeed = 10f;            // Tốc độ xoay
    public float stopOffset = 0.01f;         // Khoảng cách dừng (tránh chạm quá sâu)
    
    [Header("Heal Armor")]
    [SerializeField] bool importantArmor; // Kiểm tra áo giáp có bất tử không
    [SerializeField] int maxHealthOneArmor = 100; // Máu tối đa của một mảnh giáp
    [SerializeField] private int currentHealthOneArmor;
    
    [Header("Armor")]
    [SerializeField] List<Transform> armorParts; // Các phần của áo giáp
    [SerializeField] BoxCollider _armorCollider;

    public void ExplosionArmor()
    {
        fallSpeed = 3.5f; 
        foreach (Transform VARIABLE in armorParts)
        {
            StartCoroutine(IEDropArmor(VARIABLE));
        }
        armorParts.Clear();
        _armorCollider.enabled = false;
    }

    public void TakeDamage(int damage)
    {
        if(importantArmor)
            return;
        currentHealthOneArmor -= damage;
        if (currentHealthOneArmor <= 0)
        {
            CaculatorDropArmor();
            currentHealthOneArmor = maxHealthOneArmor;
        }
    }
    

    private void CaculatorDropArmor()
    {
        StartCoroutine(IEDropArmor(armorParts[0]));
        armorParts.Remove(armorParts[0]);
        if (armorParts.Count <= 0)
        {
            _armorCollider.enabled = false;
            Invoke(nameof(ActiveFalseThis), 5f);
        }
    }

    public void ActiveFalseThis() => gameObject.SetActive(false);
    
    IEnumerator IEDropArmor(Transform _armorPart)
    {
        _armorPart.parent = null;
        Vector3 _targetPoint = new Vector3(_armorPart.position.x,_bossParent.position.y,_armorPart.position.z);
        while (true)
        {
            _armorPart.position = Vector3.MoveTowards(_armorPart.position, _targetPoint, fallSpeed * Time.deltaTime);
            _armorPart.rotation = Quaternion.Lerp(_armorPart.rotation, Quaternion.Euler(Vector3.zero), rotationSpeed * Time.deltaTime);
            
            if (Vector3.Distance(_armorPart.position, _targetPoint) <= stopOffset)
            {
//                Debug.Log("✅ Đã tiếp đất.");
                yield break;
            }
            yield return null;
        }
    }
}