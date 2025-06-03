using System.Collections;
using UnityEngine;

public class ExplosionDor : MonoBehaviour
{
    public GameObject colliderDoor;           // Collider của cửa
    
    [Header("Cài đặt chuyển động")]
    public float launchForce = 5f;               // Lực bay ban đầu
    public float fallSpeed = 5f;                 // Tốc độ rơi (trọng lực)
    public float rotationSpeed = 180f;           // Tốc độ quay
    public LayerMask groundLayer;                // Layer mặt đất

    [Header("Door Child")]
    [SerializeField] private Transform[] _doorChild;

    [Header("Tâm để bay")]
    public Transform explosionOrigin;            // Gốc phát nổ (Transform để tính hướng bay)

    public void ExplosionDoor()
    {
        colliderDoor.SetActive(false);
        foreach (Transform door in _doorChild)
        {
            door.parent = null;

            // Tính hướng từ tâm ra
            Vector3 launchDirection = (door.position - explosionOrigin.position).normalized;

            StartCoroutine(IEDropArmor(door, launchDirection));
        }
    }

    IEnumerator IEDropArmor(Transform door, Vector3 direction)
    {
        bool isFlying = true;
        float verticalVelocity = 0f;

        while (isFlying)
        {
            // Tăng vận tốc rơi theo thời gian
            verticalVelocity -= fallSpeed * Time.deltaTime;

            // Tính chuyển động tổng hợp
            Vector3 flyMove = direction * launchForce * Time.deltaTime;
            Vector3 fallMove = new Vector3(0f, verticalVelocity, 0f) * Time.deltaTime;

            door.Translate(flyMove + fallMove, Space.World);

            // Quay vật thể trong khi bay
            door.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);

            // Raycast kiểm tra mặt đất
            if (Physics.Raycast(door.position, Vector3.down, 0.1f, groundLayer) && verticalVelocity < 0f)
            {
                Debug.DrawRay(door.position, Vector3.down * 0.1f, Color.green, 1f);
                isFlying = false;
            }
            else
            {
                Debug.DrawRay(door.position, Vector3.down * 0.1f, Color.red, 0.1f);
            }

            yield return null;
        }
    }
}
