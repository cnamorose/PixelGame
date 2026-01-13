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

    [Header("Hit Flash")]
    public float flashDuration = 0.1f;
    public int flashCount = 2;

    DevilAttackController attackController;

    [Header("Death Dialogue")]
    public DialogueSequence devilDeathDialogue;

    [Header("After Death")]
    public GameObject elevator;

    SpriteRenderer[] GetActivePhaseRenderers()
    {
        if (npcPhase1.activeSelf)
            return npcPhase1.GetComponentsInChildren<SpriteRenderer>();
        if (npcPhase2.activeSelf)
            return npcPhase2.GetComponentsInChildren<SpriteRenderer>();
        if (npcPhase3.activeSelf)
            return npcPhase3.GetComponentsInChildren<SpriteRenderer>();

        return null;
    }

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
        Debug.Log("Devil HP: " + currentHP);

        if (isTransitioning) return;

        currentHP -= damage;
        Debug.Log("Devil HP: " + currentHP);

        StartCoroutine(HitFlash());

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

    IEnumerator ShowDialogue(string text, float duration = 2f)
    {
        DialogueManager.Instance.ShowSimpleDialogueAutoClose(
            text,
            duration,
            "#AB0116"
        );
        yield return new WaitForSeconds(duration);
    }

    void Die()
    {
        if (isTransitioning) return;
        isTransitioning = true;
        Debug.Log("Devil Dead");

        StartCoroutine(DevilDeathSequence());
    }

    IEnumerator HitFlash()
    {
        SpriteRenderer[] renderers = GetActivePhaseRenderers();
        if (renderers == null) yield break;

        for (int i = 0; i < flashCount; i++)
        {
            foreach (var r in renderers)
                r.enabled = false;

            yield return new WaitForSeconds(flashDuration);

            foreach (var r in renderers)
                r.enabled = true;

            yield return new WaitForSeconds(flashDuration);
        }
    }

    IEnumerator DeathSlowMotion(float slowScale, float realTimeDuration)
    {
        float originalScale = Time.timeScale;

        Time.timeScale = slowScale;
        yield return new WaitForSecondsRealtime(realTimeDuration);

        Time.timeScale = originalScale;
    }

    IEnumerator DevilDeathSequence()
    {
        // 공격 즉시 중단
        attackController.StopAttackLoop();

        // 슬로우
        yield return StartCoroutine(DeathSlowMotion(0.15f, 0.6f));

        // 컷신 종료 후 콜백 등록
        DialogueManager.Instance.onCutsceneEnd = OnDevilDeathDialogueEnd;

        // 컷신 시작 (씬 이동 없음)
        DialogueManager.Instance.StartDialogue(devilDeathDialogue);
    }

    void OnDevilDeathDialogueEnd()
    {
        if (elevator != null)
            elevator.SetActive(true);

        StartCoroutine(DevilDisappear());
    }

    IEnumerator DevilDisappear()
    {
        yield return new WaitForSeconds(0.3f);
        gameObject.SetActive(false);
    }




}