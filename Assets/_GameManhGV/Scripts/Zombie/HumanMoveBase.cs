using System;
using UnityEngine;
using UnityEngine.Serialization;

public class HumanMoveBase : MonoBehaviour
{
    [FormerlySerializedAs("botBase")] 
    [SerializeField] protected ZombieBase zombieBase;
    [SerializeField] protected BotConfigSO BotConfigSO;
    public bool isHaveParent;
    public Transform myTrans;

#if UNITY_EDITOR
    private void OnValidate()
    {
        zombieBase = GetComponent<ZombieBase>();
    }
#endif
 
    private void Awake()
    {
        myTrans = transform;
    }

    private void Update()
    {
        CheckParent();
    }
    
    public void SetBotMove(Vector3 _point)
    {
        if (!zombieBase.IsDead)
        {
            var targetRotation = Quaternion.LookRotation(_point - myTrans.position);
            myTrans.rotation = Quaternion.Slerp(myTrans.rotation, targetRotation, 10 * Time.deltaTime);
            myTrans.position = Vector3.MoveTowards(myTrans.position, _point, BotConfigSO.moveSpeed * Time.deltaTime);
        }
    }
    
    /// <summary>
    /// Không sử dung BotConfigSO
    /// </summary>
    /// <param name="point">Điểm cần đi tới</param>
    /// <param name="speed">Tốc độ</param>
    public void SetBotMove(Vector3 point, float speed)
    {
        if (!zombieBase.IsDead)
        {
            Vector3 direction = point - myTrans.position;
            direction.y = 0f; // Loại bỏ quay theo trục Y
            if (direction != Vector3.zero) // Tránh lỗi khi hai điểm trùng nhau
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                myTrans.rotation = Quaternion.Slerp(myTrans.rotation, targetRotation, 10 * Time.deltaTime);
            }

            myTrans.position = Vector3.MoveTowards(myTrans.position, point, speed * Time.deltaTime);
        }
    }

    
    void CheckParent()
    {
        if (myTrans.parent != null)
        {
            isHaveParent = true;
        }
        else
        {
            isHaveParent = false;
        }
          
    }
}
