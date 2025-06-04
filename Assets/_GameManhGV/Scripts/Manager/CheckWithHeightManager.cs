using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckWithHeightManager : MonoBehaviour
{
    [Header("Weapon")]
    [SerializeField] private Transform Weapon81;
    [SerializeField] private Transform portraiPoint;
    [SerializeField] private Transform landScapePoint;

    private bool isPortrait; // Lưu trạng thái màn hình hiện tại

    void Start()
    {
        isPortrait = Screen.height < Screen.width;
        UpdateWeaponPosition(isPortrait);
    }

    void Update()
    {
        bool currentIsPortrait = Screen.height < Screen.width;
        if (currentIsPortrait != isPortrait)
        {
            isPortrait = currentIsPortrait;
            UpdateWeaponPosition(isPortrait);
        }
    }

    private void UpdateWeaponPosition(bool portrait)
    {
        if (portrait)
        {
            Debug.Log("Màn hình đang ở chế độ dọc (Portrait)");
            Weapon81.SetParent(portraiPoint, false);
        }
        else
        {
            Debug.Log("Màn hình đang ở chế độ ngang (Landscape)");
            Weapon81.SetParent(landScapePoint, false);
        }

        Weapon81.localPosition = Vector3.zero;
    }
}
