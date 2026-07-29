using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevilHealth : MonoBehaviour
{
    public int maxHP = 33;
    int currentHP;

    [Header("BGM")]
    public AudioClip phaseBGM;

    [Header("Phase NPCs")]
    public GameObject npcPhase1;
    public GameObject npcPhase2;
    public GameObject npcPhase3;

    [Header("Hit Flash")]
    public float flashDuration = 0.1f;
    public int flashCount = 2;

    DevilAttackController attackController;

    [Header("Death Dialogue")]
    public DialogueSequence bossDeathDialogue;
    public DialogueSequence bossDeathDialogue_EN;

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

    public void Start()
    {
        if (AudioManager.Instance != null && phaseBGM != null)
        {
            AudioManager.Instance.PlayBGM(phaseBGM);
        }
    }

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
        // ⭐ [안전장치 1] 플레이어가 이미 죽어서 게임오버 시퀀스가 돌고 있다면 데미지 및 판정 완전 무시
        if (GameOverManager.Instance != null && GameOverManager.Instance.battleResolved) return;

        if (isTransitioning) return;

        currentHP -= damage;
        Debug.Log("Devil HP: " + currentHP);

        StartCoroutine(HitFlash());

        if (currentHP <= 0)
        {
            Die();
            return;
        }

        // ----------------------------------------------------
        // ⭐ [수정 완료] 최대 체력 50에 맞춘 페이즈 전환 수치 조절
        // ----------------------------------------------------
        // 페이즈 2 전환: 체력이 30 이하로 떨어졌을 때 (기존 70 -> 30)
        if (currentHP <= 30 && currentPhase == DevilPhase.Phase1)
        {
            StartCoroutine(Phase2Transition());
        }
        // 페이즈 3 전환: 체력이 15 이하로 떨어졌을 때 (기존 40 -> 15)
        else if (currentHP <= 15 && currentPhase == DevilPhase.Phase2)
        {
            StartCoroutine(Phase3Transition());
        }
    }

    IEnumerator Phase2Transition()
    {
        isTransitioning = true;
        currentPhase = DevilPhase.Phase2;

        attackController.ForceStopAllAttacks();
        ClearRemainingProjectiles();

        // ⭐ [안전장치 2] 대화창을 띄우기 직전 한 번 더 게임오버 상태를 체크하여 꼬임 방지
        if (GameOverManager.Instance != null && GameOverManager.Instance.isGameOverSequenceRunning)
        {
            isTransitioning = false;
            yield break;
        }

        yield return ShowDialogue("이렇게 쎄다고?");

        npcPhase1.SetActive(false);
        npcPhase2.SetActive(true);

        yield return new WaitForSeconds(1f);

        // 다시 루프 돌기 전 플레이어가 죽었는지 최종 확인
        if (GameOverManager.Instance != null && GameOverManager.Instance.isGameOverSequenceRunning) yield break;

        attackController.BeginAttackLoop();
        isTransitioning = false;
    }

    IEnumerator Phase3Transition()
    {
        isTransitioning = true;
        currentPhase = DevilPhase.Phase3;

        attackController.ForceStopAllAttacks();
        ClearRemainingProjectiles();

        // ⭐ [안전장치 3] 대화창 띄우기 전 체크
        if (GameOverManager.Instance != null && GameOverManager.Instance.isGameOverSequenceRunning)
        {
            isTransitioning = false;
            yield break;
        }

        yield return ShowDialogue("아직.. 끝나지 않았다..");

        npcPhase2.SetActive(false);
        npcPhase3.SetActive(true);

        yield return new WaitForSeconds(1f);

        if (GameOverManager.Instance != null && GameOverManager.Instance.isGameOverSequenceRunning) yield break;

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
        // ⭐ [안전장치 4] 플레이어와 승부 선점 처리
        var gom = GameOverManager.Instance;
        if (gom != null)
        {
            if (gom.battleResolved) return;   // 플레이어가 먼저 죽음 → 보스 사망 연출 차단
            gom.battleResolved = true;        // 보스가 먼저 → 승부 확정 (게임오버를 막음)
        }

        if (isTransitioning) return;
        isTransitioning = true;
        attackController.ForceStopAllAttacks();
        ClearRemainingProjectiles();

        // 보스가 먼저 완벽하게 승리 판정을 굳혔으므로 플레이어 조작을 잠궈 무적(데미지 차단) 상태로 만듭니다.
        PlayerAction player = FindObjectOfType<PlayerAction>();
        if (player != null)
        {
            player.LockControl();
        }
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
        attackController.ForceStopAllAttacks();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopBGM();
            AudioManager.Instance.StopAllSFX();
        }

        yield return StartCoroutine(DeathSlowMotion(0.15f, 0.6f));

        DialogueManager.Instance.onCutsceneEnd = OnDevilDeathDialogueEnd;

        DialogueSequence selectedDialogue =
            GameManager_L.Instance.currentLanguage == Language.EN
            && bossDeathDialogue_EN != null
                ? bossDeathDialogue_EN
                : bossDeathDialogue;

        DialogueManager.Instance.StartDialogue(selectedDialogue);
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

        attackController.ForceStopAllAttacks();

        gameObject.SetActive(false);
    }

    void ClearRemainingProjectiles()
    {
        GameObject[] projectiles = GameObject.FindGameObjectsWithTag("BossProjectile");
        foreach (GameObject p in projectiles)
        {
            Destroy(p);
        }
        Debug.Log("화면의 모든 데빌 투사체 제거 완료");
    }

    public void ForceCleanupForGameOver()
    {
        if (attackController != null)
            attackController.ForceStopAllAttacks();   // 공격 루프 정지 → SkyAttackLoop 멈춤 → 소리 안 남

        ClearRemainingProjectiles();

        if (AudioManager.Instance != null)
            AudioManager.Instance.StopAllSFX();
    }
}