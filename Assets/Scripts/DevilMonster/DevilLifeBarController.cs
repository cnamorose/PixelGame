using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevilLifeBarController : MonoBehaviour
{
    [Header("Boss HP")]
    public int maxHP = 100;
    public int currentHP = 100;

    [Header("Life Bar Sprites")]
    public Sprite hp100Sprite; // 100 ~ 71
    public Sprite hp70Sprite;  // 70 ~ 41
    public Sprite hp40Sprite;  // 40 ~ 11
    public Sprite hp10Sprite;  // 10 ~ 1
    public Sprite hp0Sprite;   // 0

    SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        currentHP = maxHP;
        UpdateLifeBar();
    }

    /// <summary>
    /// 보스 HP 감소 (몬스터 처치 시 호출)
    /// </summary>
    public void ReduceHP(int amount)
    {
        currentHP -= amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        UpdateLifeBar();
    }

    void UpdateLifeBar()
    {
        if (currentHP > 70)
        {
            sr.sprite = hp100Sprite;
        }
        else if (currentHP > 40)
        {
            sr.sprite = hp70Sprite;
        }
        else if (currentHP > 10)
        {
            sr.sprite = hp40Sprite;
        }
        else if (currentHP > 0)
        {
            sr.sprite = hp10Sprite;
        }
        else
        {
            sr.sprite = hp0Sprite;
            OnBossPhaseEnd();
        }
    }

    void OnBossPhaseEnd()
    {
        Debug.Log("Devil Phase 1 종료");

        // 여기서:
        // - 대사 출력
        // - 연출
        // - Phase2 씬 로드
        // SceneManager.LoadScene("DevilPhase2");
    }
}
