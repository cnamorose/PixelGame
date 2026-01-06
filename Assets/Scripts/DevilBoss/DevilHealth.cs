using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevilHealth : MonoBehaviour
{
    public int maxHP = 100;
    int currentHP;

    [Header("Phase NPCs")]
    public GameObject npcPhase1;
    public GameObject npcPhase2;
    public GameObject npcPhase3;

    DevilAttackController attackController;

    enum DevilPhase
    {
        Phase1,
        Phase2,
        Phase3
    }

    DevilPhase currentPhase = DevilPhase.Phase1;
    bool isTransitioning = false;

    void Awake()
    {
        currentHP = maxHP;
        attackController = GetComponent<DevilAttackController>();

        // 초기 페이즈
        npcPhase1.SetActive(true);
        npcPhase2.SetActive(false);
        npcPhase3.SetActive(false);
    }

    public void TakeDamage(int damage)
    {
        if (isTransitioning) return;

        currentHP -= damage;
        Debug.Log("Devil HP: " + currentHP);

        if (currentHP <= 0)
        {
            Die();
            return;
        }

        // 페이즈 체크
        if (currentHP <= 70 && currentPhase == DevilPhase.Phase1)
        {
            StartCoroutine(Phase2Transition());
        }
        else if (currentHP <= 40 && currentPhase == DevilPhase.Phase2)
        {
            StartCoroutine(Phase3Transition());
        }
    }

    IEnumerator Phase2Transition()
    {
        isTransitioning = true;
        currentPhase = DevilPhase.Phase2;

        // 공격 완전 정지
        attackController.StopAttackLoop();

        // 단발 대사
        yield return ShowDialogue("이렇게 쎄다고?");

        // NPC 교체
        npcPhase1.SetActive(false);
        npcPhase2.SetActive(true);

        // 공격 재개
        attackController.BeginAttackLoop();

        isTransitioning = false;
    }

    IEnumerator Phase3Transition()
    {
        isTransitioning = true;
        currentPhase = DevilPhase.Phase3;

        attackController.StopAttackLoop();

        yield return ShowDialogue("아직 끝나지 않았다");

        npcPhase2.SetActive(false);
        npcPhase3.SetActive(true);

        attackController.BeginAttackLoop();

        isTransitioning = false;
    }

    IEnumerator ShowDialogue(string text)
    {
        Debug.Log(text);
        yield return new WaitForSeconds(2f);
    }

    void Die()
    {
        Debug.Log("Devil Dead");

        attackController.StopAttackLoop();
        gameObject.SetActive(false);
    }
}