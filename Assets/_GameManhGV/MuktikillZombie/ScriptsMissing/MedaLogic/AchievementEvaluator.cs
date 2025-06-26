using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementEvaluator : Singleton<AchievementEvaluator>
{
    protected override void Awake()
    {
        base.Awake();
        canRun = true;
    }

    [Serializable]
    public struct MedalDetail
    {
        public float Time;    // Thời gian kiểm tra lại
        public int Kill;      // Số lượng kill yêu cầu
        public float Point;   // Điểm yêu cầu
        public string MedalName; // Tên huy chương
    }

    public MedalsUI _medalsUI;

    [Header("Cấu hình huy chương")]
    public List<MedalDetail> medalDetails = new List<MedalDetail>
    {
        new MedalDetail
        {
            MedalName = "GOOD JOB",
            Time = 3f,
            Kill = 3,
            Point = 3f
        },
        new MedalDetail
        {
            MedalName = "WELL DONE",
            Time = 3f,
            Kill = 4,
            Point = 5f
        },
        new MedalDetail
        {
            MedalName = "GREAT",
            Time = 4f,
            Kill = 5,
            Point = 6f
        },
        new MedalDetail
        {
            MedalName = "EXCELLENT",
            Time = 5f,
            Kill = 6,
            Point = 8f
        },
        new MedalDetail
        {
            MedalName = "PERFECT",
            Time = 6f,
            Kill = 7,
            Point = 10f
        }
    };

    [Header("Thời gian đợi giữa các huy chương (giây)")]
    public float medalCooldown = 3f; // Thời gian đợi truyền từ Inspector
    private bool canRun;
    public float delayCheck = 1f;
    public float debuffDuration = 3f;
    public float reductionValue = 0.5f;
    public float maxPoint = 10f;

    private List<KillInfo> _cachedKills = new List<KillInfo>();
    private float _debuffTime;
    private Coroutine _checkCoroutine;

    private float _nextMedalAvailableTime = 0f; // Thời gian bắt đầu nhận kill tiếp theo

    [Serializable]
    private struct KillInfo
    {
        public float Time;
        public float Point;
    }

    public void OnBotKilled(float point = 1f, bool isSubBoss = false, bool isBoss = false, bool isSpecialBoss = false)
    {
        if (Time.time < _nextMedalAvailableTime)
        {
            // Đang trong thời gian đợi, không nhận kill mới
            Debug.Log("⏳ Đang trong thời gian đợi huy chương, kill không được tính");
            return;
        }

        // Ưu tiên các loại bot đặc biệt
        if (isSpecialBoss)
        {
            GrantSpecialMedal("PERFECT");
            return;
        }
        if (isBoss)
        {
            GrantSpecialMedal("EXCELLENT");
            return;
        }
        if (isSubBoss)
        {
            GrantSpecialMedal("GREAT");
            return;
        }

        AddKill(point);
    }

    private void GrantSpecialMedal(string medalName)
    {
        Debug.Log($"🏅 Huy chương: {medalName}");
        _nextMedalAvailableTime = Time.time + medalCooldown;
        _cachedKills.Clear(); // Reset kills khi nhận huy chương đặc biệt
    }

    private void AddKill(float point)
    {
        if(!canRun)
            return;
        
        if (Time.time < _nextMedalAvailableTime)
            return;

        bool underDebuff = _debuffTime > Time.time;

        _cachedKills.Add(new KillInfo
        {
            Point = point * (underDebuff ? reductionValue : 1f),
            Time = Time.time
        });

        if (_checkCoroutine != null)
            StopCoroutine(_checkCoroutine);

        _checkCoroutine = StartCoroutine(CheckAchievementDelay());
    }

    private IEnumerator CheckAchievementDelay()
    {
        yield return new WaitForSeconds(delayCheck);
        CheckAchievement();
    }

    private void CheckAchievement()
    {
        float currentTime = Time.time;
        int killCount = 0;
        float pointSum = 0;
        int medalIndex = medalDetails.Count - 1;

        for (int i = _cachedKills.Count - 1; i >= 0; i--)
        {
            if (_cachedKills[i].Time >= currentTime - medalDetails[medalIndex].Time)
            {
                killCount++;
                pointSum += _cachedKills[i].Point;
                pointSum = Mathf.Clamp(pointSum, 0, maxPoint);
            }
            else
            {
                if (killCount >= medalDetails[medalIndex].Kill && pointSum >= medalDetails[medalIndex].Point)
                {
                    GrantMedal(medalIndex);
                    return;
                }

                medalIndex--;
                i++; // Kiểm tra lại killInfo này với điều kiện tiếp theo

                if (medalIndex < 0)
                {
                    _cachedKills.RemoveRange(0, i);
                    return;
                }
            }
        }

        for (; medalIndex >= 0; medalIndex--)
        {
            if (killCount >= medalDetails[medalIndex].Kill && pointSum >= medalDetails[medalIndex].Point)
            {
                GrantMedal(medalIndex);
                return;
            }
        }
    }

    public void GrantMedal(int index)
    {
        var medal = medalDetails[index];
        _medalsUI.OnGetMedal(index);
        Debug.Log($"🏅 Huy chương đạt được: {medal.MedalName}");

        _debuffTime = Time.time + debuffDuration;
        _nextMedalAvailableTime = Time.time + medalCooldown; // Kích hoạt thời gian đợi sau khi nhận huy chương

        if (index == medalDetails.Count - 1)
        {
            // Nếu là huy chương cao nhất, nhận luôn và reset kills
            _cachedKills.Clear();
        }
        else
        {
            // Nếu không phải huy chương cao nhất, giảm điểm kill còn lại
            ReduceCachedPoints();
        }
    }

    private void ReduceCachedPoints()
    {
        for (int i = 0; i < _cachedKills.Count; i++)
        {
            var kill = _cachedKills[i];
            kill.Point *= reductionValue;
            _cachedKills[i] = kill;
        }
    }
    public void ResetKillData()
    {
        canRun = false;
        _cachedKills.Clear();               // Xóa danh sách kill
        _nextMedalAvailableTime = 10f;
        if (_checkCoroutine != null)
        {
            StopCoroutine(_checkCoroutine);
            _checkCoroutine = null;
        }
        Debug.Log("🔄 Reset toàn bộ kill, điểm và trạng thái về mặc định.");
    }
    
}
