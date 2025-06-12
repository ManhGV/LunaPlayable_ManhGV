using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : Singleton<PlayerHP>
{
    [SerializeField] private Image HPimage;
    [SerializeField] private Gradient HPState;
    [SerializeField] private Text HPTxt;
    [SerializeField] private float HPMax;
    [SerializeField] private float HPPoint;
    private Queue<float> damageQueue = new Queue<float>(); // Hàng đợi lưu trữ sát thương
    private bool isProcessingDamage = false;

    // [Header("Healing Effect")]
    // [SerializeField] int numberOfHeals = 10; // Số lần hồi phục
    // [SerializeField] int percentPerHeal = 50; //%máu hồi phục
    // [SerializeField] float totalDuration = 2.28f; //số giây hồi phục xong
    // private Coroutine healingCoroutine;
    private void OnEnable()
    {
        HPPoint = HPMax;
        HPTxt.text = HPMax.ToString();
        EventManager.AddListener<float>(EventName.OnTakeDamagePlayer, OnTakeDamagePlayer);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener<float>(EventName.OnTakeDamagePlayer, OnTakeDamagePlayer);
    }

    private void OnTakeDamagePlayer(float Damage)
    {
//        print("TakeDamagePlayer: " + Damage);
        // Đưa lượng sát thương vào hàng đợi để xử lý
        damageQueue.Enqueue(Damage);
        EffectUI.Instance.Play();
        print("Take");
        // Nếu chưa có Coroutine xử lý sát thương, bắt đầu một Coroutine mới
        if (!isProcessingDamage)
            StartCoroutine(ProcessDamageQueue());
    }

    public void ClearListDamage() => damageQueue.Clear();

    bool endGame = false;

    private IEnumerator ProcessDamageQueue()
    {
        isProcessingDamage = true;
        // Duyệt qua hàng đợi sát thương và xử lý từng lượng sát thương một
        while (!endGame && damageQueue.Count > 0)
        {
            float damage = damageQueue.Dequeue();

            // Trừ lượng máu đúng với lượng sát thương trong hàng đợi
            HPPoint -= damage;
            float HPPoint__ = HPPoint;
            HPPoint = Mathf.Max(HPPoint, 0); // Đảm bảo HP không giảm dưới 0
            // if (HPPoint <= 15)
            // {
            //     if(healingCoroutine== null)
            //         healingCoroutine = StartCoroutine(IEHealing(percentPerHeal,numberOfHeals, totalDuration));
            // }
            // Cập nhật giao diện
            HPTxt.text = HPPoint.ToString();
            HPimage.fillAmount = HPPoint / HPMax;

            HPimage.color = HPState.Evaluate(HPimage.fillAmount);
            // Tạo một khoảng nghỉ nhỏ để tránh trừ sát thương quá nhanh, điều này có thể điều chỉnh tùy ý
            if (HPPoint__ <= 0)
            {
                endGame = true;
                GameManager.Instance.EndGame(false);
            }

            yield return new WaitForSeconds(0.2f);
        }

        isProcessingDamage = false;
    }
    //
    // public IEnumerator IEHealing(float percentTotalHeal, int numberOfHeals, float totalDuration)
    // {
    //     healingEffect.SetTrigger("Play");
    //
    //     if (numberOfHeals <= 0 || percentTotalHeal <= 0f)
    //         yield break;
    //
    //     float intervalSeconds = totalDuration / numberOfHeals;
    //     float percentPerHeal = percentTotalHeal / numberOfHeals;
    //     int healAmountPerTick = Mathf.RoundToInt(HPMax * percentPerHeal / 100f);
    //
    //     for (int i = 0; i < numberOfHeals; i++)
    //     {
    //         if (HPPoint >= HPMax)
    //             yield break;
    //
    //         HPPoint = Mathf.Min(HPPoint + healAmountPerTick, HPMax);
    //         //EventManager.Invoke(EventName.OnPlayerDead, HPPoint <= 0);
    //
    //         HPimage.fillAmount = (float)HPPoint / HPMax;
    //         HPimage.color = HPState.Evaluate(HPimage.fillAmount);
    //
    //         yield return new WaitForSeconds(intervalSeconds);
    //     }
    //
    //     healingCoroutine = null;
    // }
}