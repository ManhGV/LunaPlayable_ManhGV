using System;
using UnityEngine;

public class HumanMoveBase : MonoBehaviour
{
    [SerializeField] protected BotNetwork botNet;
    [SerializeField] protected BotConfigSO BotConfigSO;
    public bool isHaveParent;
    public Transform myTrans;

 
    private void Awake()
    {
        myTrans = transform;
    }
    protected virtual void OnEnable()
    {
        botNet.OnBotDead += OnBotDead;
    }
    
    protected virtual void OnDisable()
    {
        botNet.OnBotDead -= OnBotDead;
    }

    private void Update()
    {
        CheckParent();
    }
    
    private void OnBotDead()
    {
        
    }
    
    public void SetBotMove(Transform point)
    {
        if (!botNet.IsDead)
        {
            var targetRotation = Quaternion.LookRotation(point.position - myTrans.position);
            myTrans.rotation = Quaternion.Slerp(myTrans.rotation, targetRotation, 10 * Time.deltaTime);
            myTrans.position = Vector3.MoveTowards(myTrans.position, point.position, BotConfigSO.moveSpeed * Time.deltaTime);
        }
    }
    
    /// <summary>
    /// Không sử dung BotConfigSO
    /// </summary>
    /// <param name="point">Điểm cần đi tới</param>
    /// <param name="speed">Tốc độ</param>
    public void SetBotMove(Transform point, float speed)
    {
        if (!botNet.IsDead)
        {
            Vector3 direction = point.position - myTrans.position;
            direction.y = 0f; // Loại bỏ quay theo trục Y
            if (direction != Vector3.zero) // Tránh lỗi khi hai điểm trùng nhau
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                myTrans.rotation = Quaternion.Slerp(myTrans.rotation, targetRotation, 10 * Time.deltaTime);
            }

            myTrans.position = Vector3.MoveTowards(myTrans.position, point.position, speed * Time.deltaTime);
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
