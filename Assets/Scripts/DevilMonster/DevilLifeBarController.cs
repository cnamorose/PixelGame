using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DevilLifeBarController : MonoBehaviour
{
    [Header("Phase End Dialogue")]
    public DialogueSequence devilPhase1EndDialogue;

    // ⭐ [수정] 최대 체력을 40으로 변경
    [Header("Boss HP")]
    public int maxHP = 40;
    public int currentHP = 40;

    // ⭐ [참고] 최대 체력 40 기준 새로운 구간 설명
    [Header("Life Bar Sprites")]
    public Sprite hp100Sprite; // 40 ~ 31 (체력 75% 초과)
    public Sprite hp70Sprite;  // 30 ~ 21 (체력 50% 초과 ~ 75% 이하)
    public Sprite hp40Sprite;  // 20 ~ 11 (체력 25% 초과 ~ 50% 이하)
    public Sprite hp10Sprite;  // 10 ~ 1  (체력 0% 초과 ~ 25% 이하)
    public Sprite hp0Sprite;   // 0       (사망)

    SpriteRenderer sr;

    public bool isBossDead = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        currentHP = maxHP;
        UpdateLifeBar();
    }

    public void ReduceHP(int amount)
    {
        if (isBossDead) return;

        currentHP -= amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        UpdateLifeBar();
    }

    // ⭐ [핵심 수정] 최대 체력 40에 맞춘 스프라이트 분기 수치 수정
    void UpdateLifeBar()
    {
        if (currentHP > 30) // 기존 70 -> 30 (75% 지점)
        {
            sr.sprite = hp100Sprite;
        }
        else if (currentHP > 20) // 기존 40 -> 20 (50% 지점)
        {
            sr.sprite = hp70Sprite;
        }
        else if (currentHP > 10) // 기존 10 -> 10 (25% 지점)
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

            if (!isBossDead)
            {
                isBossDead = true;
                OnBossPhaseEnd();
            }
        }
    }

    void OnBossPhaseEnd()
    {
        DevilPhaseManager phase =
            FindObjectOfType<DevilPhaseManager>();

        if (phase != null)
            phase.StartPhaseEnd();
    }
}