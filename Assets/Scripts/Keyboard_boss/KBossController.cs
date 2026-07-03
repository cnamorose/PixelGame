using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KBossController : MonoBehaviour
{
    public enum A4AttackShape { Cross, XShape }

    [Header("Drop")]
    public GameObject keyPrefab;

    [Header("Death Animation")]
    public AnimationClip deathClip;

    [Header("Hit Effect")]
    public float hitBlinkDuration = 0.15f;
    public int hitBlinkCount = 3;

    SpriteRenderer sr;
    bool isHitBlinking = false;

    [Header("Status")]
    public int maxHp = 25;
    public int hp;

    [Header("Attack")]
    public float attackDelay = 1.0f;
    public float idleDelay = 1.0f;

    [Header("References")]
    public Animator anim;

    [Header("Attack Positions")]
    public Transform defaultPos;
    public Transform attack2Pos;

    [Header("Scale Settings")]
    public float attack2ScaleMultiplier = 4f;

    [Header("A1 Move Positions")]
    public Transform a1StartPos;
    public Transform a1EndPos;

    // ============================================================
    // 🔄 [A4 지그재그 패턴 변수 대폭 수정] 이제 배열 노가다가 필요 없습니다!
    // ============================================================
    [Header("A4 ZigZag Slide Settings")]
    public float a4AttackDuration = 5f;    // A4 지그재그 패턴이 유지되는 총 시간
    public float slideSpeed = 6f;          // 좌우 왕복할 때의 속도 (기존 10에서 현실적으로 하향)
    public float attackInterval = 0.4f;    // 정박 탄막이 나오는 텀 (플레이어가 피할 틈을 줌)
    [Tooltip("화면 가장자리에서 얼마나 안쪽으로 들어와서 왕복할지 설정 (카메라 가로 반경 대비 비율, 0.7~0.8 추천)")]
    public float horizontalRangeRatio = 0.75f;

    [Header("A4 Custom Shape Settings")]
    [Tooltip("Cross = + 형태, XShape = X 형태")]
    public A4AttackShape a4Shape = A4AttackShape.Cross;

    [Header("Move Settings")]
    public float moveSpeed = 5f;

    [Header("Target")]
    public Transform player;

    private Vector3 originalScale;

    [Header("Throw Settings")]
    public GameObject throwPrefab;
    public Transform[] throwPoints;
    public float minThrowSpeed = 10f;
    public float maxThrowSpeed = 14f;
    public float throwForce = 7f;
    public float spreadAngle = 60f;

    [Header("A2 Drop Settings")]
    public GameObject dropPrefab;
    public float dropInterval = 0.8f;
    public float dropHeight = 6f;
    public float dropRangeX = 6f;

    [Header("Dialogue")]
    public DialogueSequence bossDeathDialogue;
    public DialogueSequence bossDeathDialogue_EN;

    private bool isDead = false;
    private bool isAttacking = false;
    private int lastAttackIndex = -1;
    bool isA2Active = false;
    Cameramove camMove;

    void Start()
    {
        hp = maxHp;
        if (anim == null) anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;
        camMove = Camera.main.GetComponent<Cameramove>();
    }

    public void StartBoss()
    {
        if (isDead || isAttacking) return;
        StartCoroutine(BossRoutine());
    }

    void LateUpdate()
    {
        if (!isA2Active) return;
        Camera cam = Camera.main;
        if (cam == null || sr == null) return;

        float camTopY = cam.transform.position.y + cam.orthographicSize;
        float spriteTopOffset = sr.bounds.max.y - transform.position.y;

        Vector3 pos = transform.position;
        pos.y = camTopY - spriteTopOffset;
        transform.position = pos;
    }

    Transform GetPlayer()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
        return player;
    }

    public int burstCount = 5;

    void ThrowFromIndex(int index)
    {
        // ⭐ [핵심 추가] 현재 실행 중인 패턴이 4번(지그재그)일 때는 1번 애니메이션 이벤트를 무시합니다!
        if (lastAttackIndex == 4) return;

        if (throwPrefab == null || throwPoints == null)
            return;

        if (index < 0 || index >= throwPoints.Length)
            return;

        Transform point = throwPoints[index];
        if (point == null) return;

        int throwCount = Random.Range(1, 3);

        for (int i = 0; i < throwCount; i++)
        {
            GameObject obj = Instantiate(throwPrefab, point.position, Quaternion.identity);
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            if (rb == null) continue;

            float angle = Random.Range(0f, 360f);
            Vector2 dir = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            ).normalized;

            float speed = Random.Range(minThrowSpeed, maxThrowSpeed);
            rb.velocity = dir * speed;
        }
    }

    public void ThrowRight() { ThrowFromIndex(1); }
    public void ThrowLeft() { ThrowFromIndex(0); }

    // ⭐ [A4 탄막 스폰 위치 버그 수정] 이제 targetPoint가 아니라 '보스의 현재 실시간 좌표'에서 뿜어나갑니다!
    void A4ShapeThrowFromPosition(Vector3 spawnPosition)
    {
        if (throwPrefab == null) return;

        float startAngle = (a4Shape == A4AttackShape.Cross) ? 0f : 45f;

        for (int i = 0; i < 4; i++)
        {
            float targetAngle = startAngle + (i * 90f);
            GameObject obj = Instantiate(throwPrefab, spawnPosition, Quaternion.identity);
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            if (rb == null) continue;

            Vector2 dir = new Vector2(Mathf.Cos(targetAngle * Mathf.Deg2Rad), Mathf.Sin(targetAngle * Mathf.Deg2Rad)).normalized;
            float speed = Random.Range(minThrowSpeed, maxThrowSpeed);
            rb.velocity = dir * speed;
        }
    }

    Vector3 GetRandomDropPositionInCamera()
    {
        Camera cam = Camera.main;
        if (cam == null) return transform.position;

        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;
        Vector3 camPos = cam.transform.position;

        float minX = camPos.x - camWidth;
        float maxX = camPos.x + camWidth;
        float topY = camPos.y + camHeight;
        float spawnYOffset = 0.5f;

        return new Vector3(Random.Range(minX, maxX), topY + spawnYOffset, 0f);
    }

    void DropFromSky()
    {
        if (!isA2Active || dropPrefab == null) return;
        Vector3 spawnPos = GetRandomDropPositionInCamera();
        Instantiate(dropPrefab, spawnPos, Quaternion.identity);
    }

    IEnumerator DropRoutine(float duration)
    {
        float startTime = Time.time;
        while (Time.time - startTime < duration && !isDead)
        {
            DropFromSky();
            yield return new WaitForSeconds(dropInterval);
        }
    }

    IEnumerator MoveToPosition(Vector3 targetPos)
    {
        while (Vector3.Distance(transform.position, targetPos) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
    }

    IEnumerator BossRoutine()
    {
        while (!isDead)
        {
            if (isAttacking) { yield return null; continue; }

            yield return new WaitForSeconds(attackDelay);
            int attackIndex = GetRandomAttack();
            Debug.Log("AttackIndex 선택됨: " + attackIndex);

            yield return StartCoroutine(DoAttack(attackIndex));
        }
    }

    int GetRandomAttack()
    {
        int index;
        do { index = Random.Range(1, 5); } while (index == lastAttackIndex);
        lastAttackIndex = index;
        return index;
    }

    IEnumerator DoAttack(int attackIndex)
    {
        Collider2D bossCollider = GetComponent<Collider2D>();

        if (attackIndex == 1)
        {
            if (defaultPos != null) transform.position = defaultPos.position;
            anim.SetInteger("AttackIndex", 1);
            anim.SetBool("IsAttacking", true);

            float startTime = Time.time;
            while (Time.time - startTime < 5f && !isDead) yield return null;

            anim.SetBool("IsAttacking", false);
            anim.SetInteger("AttackIndex", 0);
            isAttacking = false;
            yield break;
        }

        if (attackIndex == 2)
        {
            isA2Active = true;
            if (bossCollider != null) bossCollider.enabled = false;

            transform.position = GetA2CameraTopPosition();
            anim.SetInteger("AttackIndex", 2);
            anim.SetBool("IsAttacking", true);

            Coroutine dropCo = StartCoroutine(DropRoutine(5f));
            if (camMove != null) StartCoroutine(camMove.ShakeCamera(5f, 0.15f));

            float startTime = Time.time;
            while (Time.time - startTime < 5f && !isDead) yield return null;

            StopCoroutine(dropCo);
            anim.SetBool("IsAttacking", false);
            anim.SetInteger("AttackIndex", 0);

            if (defaultPos != null) transform.position = defaultPos.position;
            if (bossCollider != null) bossCollider.enabled = true;

            isA2Active = false;
            isAttacking = false;
            yield break;
        }

        if (attackIndex == 3)
        {
            if (a1StartPos != null) transform.position = a1StartPos.position;
            anim.SetInteger("AttackIndex", 3);
            anim.SetBool("IsAttacking", true);

            if (a1EndPos != null)
            {
                while (Vector3.Distance(transform.position, a1EndPos.position) > 0.05f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, a1EndPos.position, moveSpeed * Time.deltaTime);
                    yield return null;
                }
                transform.position = a1EndPos.position;
            }

            anim.SetBool("IsAttacking", false);
            anim.SetInteger("AttackIndex", 0);

            if (!isDead && defaultPos != null) transform.position = defaultPos.position;
            isAttacking = false;
            yield break;
        }

        // =========================================================================
        // 🔴 [완벽 개조] A4: 카메라 상단 고정, 규칙적이고 아름다운 좌우 지그재그 왕복 패턴
        // =========================================================================
        if (attackIndex == 4)
        {
            isAttacking = true;
            anim.SetInteger("AttackIndex", 1);
            anim.SetBool("IsAttacking", true);

            // 1. 카메라 해상도 기반 화면 좌/우 끝점 계산
            Camera cam = Camera.main;
            float camHeight = cam.orthographicSize;
            float camWidth = camHeight * cam.aspect;
            float camCenterX = cam.transform.position.x;
            float targetY = cam.transform.position.y + (camHeight * 0.3f); // 화면 상단 고정

            float leftLimitX = camCenterX - (camWidth * horizontalRangeRatio);
            float rightLimitX = camCenterX + (camWidth * horizontalRangeRatio);

            // 2. 시작 시 우선 왼쪽 끝점으로 이동 (이때는 이동만 함)
            Vector3 startPosition = new Vector3(leftLimitX, targetY, transform.position.z);
            yield return StartCoroutine(MoveToPosition(startPosition));

            // 시작점 도착했으니 정박 탄막 1회 팡!
            A4ShapeThrowFromPosition(transform.position);
            yield return new WaitForSeconds(attackInterval); // 발사 후 살짝 멈춰서 플레이어에게 대비할 틈을 줌

            float patternStartTime = Time.time;
            bool movingRight = true; // 우측으로 갈 차례

            // 3. 패턴 유지 시간 동안 지그재그 왕복
            while (Time.time - patternStartTime < a4AttackDuration && !isDead)
            {
                float targetX = movingRight ? rightLimitX : leftLimitX;
                Vector3 targetDestination = new Vector3(targetX, targetY, transform.position.z);

                // 목표 지점을 향해 이동
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetDestination,
                    slideSpeed * Time.deltaTime
                );

                // ⭐ [핵심 변경] 이동 중엔 쏘지 않고, 목표 끝점에 완벽히 도착했을 때만 발사!
                if (Mathf.Abs(transform.position.x - targetX) <= 0.05f)
                {
                    transform.position = targetDestination; // 좌표 정밀 고정

                    // 끝점에서 십자(+) 혹은 X자 탄막 팡!
                    A4ShapeThrowFromPosition(transform.position);

                    // 발사 후 딜레이 대기 (이게 있어야 플레이어가 쏟아지는 탄 줄기를 보고 반응합니다)
                    yield return new WaitForSeconds(attackInterval);

                    // 방향 전환
                    movingRight = !movingRight;
                }

                yield return null;
            }

            anim.SetBool("IsAttacking", false);
            anim.SetInteger("AttackIndex", 0);

            // 패턴 종료 후 기본 위치로 안전 복귀
            if (!isDead && defaultPos != null)
            {
                yield return StartCoroutine(MoveToPosition(defaultPos.position));
            }

            isAttacking = false;
            yield break;
        }
    }

    Vector3 GetA2CameraTopPosition()
    {
        Camera cam = Camera.main;
        if (cam == null || sr == null) return transform.position;

        float camTopY = cam.transform.position.y + cam.orthographicSize;
        float camCenterX = cam.transform.position.x;
        float spriteTopOffset = sr.bounds.max.y - transform.position.y;

        return new Vector3(camCenterX, camTopY - spriteTopOffset, transform.position.z);
    }

    public void TakeDamage(int damage)
    {
        if (GameOverManager.Instance != null && GameOverManager.Instance.isGameOverSequenceRunning) return;
        if (isDead || isA2Active) return;

        hp -= damage;
        if (!isHitBlinking) StartCoroutine(HitBlink());

        if (hp <= 0)
        {
            hp = 0;
            isDead = true;
            Die();
        }
    }

    IEnumerator HitBlink()
    {
        isHitBlinking = true;
        for (int i = 0; i < hitBlinkCount; i++)
        {
            sr.color = new Color(1f, 1f, 1f, 0.2f);
            yield return new WaitForSeconds(hitBlinkDuration);
            sr.color = Color.white;
            yield return new WaitForSeconds(hitBlinkDuration);
        }
        sr.color = Color.white;
        isHitBlinking = false;
    }

    void Die()
    {
        if (GameOverManager.Instance != null && GameOverManager.Instance.isGameOverSequenceRunning) return;
        isDead = true;

        if (AudioManager.Instance != null) AudioManager.Instance.StopBGM();
        ClearRemainingProjectiles();

        Vector3 deathPos = transform.position;
        StopAllCoroutines();
        isAttacking = false;
        isA2Active = false;
        transform.position = deathPos;

        PlayerAction player = FindObjectOfType<PlayerAction>();
        if (player != null) player.LockControl();

        anim.enabled = true;
        anim.speed = 1f;
        anim.Rebind();
        anim.Update(0f);
        anim.Play("C_Boss_E", 0, 0f);

        StartCoroutine(DestroyAfterDeath());
    }

    void ClearRemainingProjectiles()
    {
        GameObject[] projectiles = GameObject.FindGameObjectsWithTag("BossProjectile");
        foreach (GameObject p in projectiles) Destroy(p);
        Debug.Log("모든 공격 프리팹 제거 완료");
    }

    [SerializeField] float keyDropYOffset = -0.5f;

    IEnumerator DestroyAfterDeath()
    {
        PlayerAction player = FindObjectOfType<PlayerAction>();
        Time.timeScale = 0.25f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        anim.speed = 0.5f;

        DialogueManager.Instance.onCutsceneEnd = () =>
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
            StartCoroutine(FinishDeathAnimation(player));
        };

        DialogueSequence selectedDialogue =
            GameManager_L.Instance.currentLanguage == Language.EN && bossDeathDialogue_EN != null
                ? bossDeathDialogue_EN
                : bossDeathDialogue;

        DialogueManager.Instance.StartDialogue(selectedDialogue);
        yield break;
    }

    IEnumerator FinishDeathAnimation(PlayerAction player)
    {
        anim.speed = 2.5f;
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);

        if (state.normalizedTime < 1f)
        {
            float remainingTime = (1f - state.normalizedTime) * state.length;
            yield return new WaitForSecondsRealtime(remainingTime);
        }

        if (keyPrefab != null)
        {
            Vector3 dropPos = transform.position;
            dropPos.y += keyDropYOffset;
            Instantiate(keyPrefab, dropPos, Quaternion.identity);
        }

        if (player != null) player.UnlockControl();
        Destroy(gameObject);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (isDead) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerAction pAction = collision.gameObject.GetComponent<PlayerAction>();
            if (pAction != null) pAction.TakeDirectDamage();
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (isDead) return;
        if (other.CompareTag("Player"))
        {
            PlayerAction pAction = other.GetComponent<PlayerAction>();
            if (pAction != null) pAction.TakeDirectDamage();
        }
    }
}