using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KBossController : MonoBehaviour
{
    // ⭐ [A4 전용 탄막 형태 선택 변수]
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
    public int maxHp = 40;
    public int hp;

    [Header("Attack")]
    public float attackDelay = 1.0f;   // 공격 전 대기
    public float idleDelay = 1.0f;      // 공격 후 대기

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
    // ⭐ [A4 패턴 설정 변수]
    // ============================================================
    [Header("A4 Multi-Point Slide Settings")]
    public Transform[] a4SpawnPoints;     // 4개의 이동 지점 배열
    public float a4AttackDuration = 6f;    // A4 공격 패턴이 유지되는 총 시간
    public float slideSpeed = 10f;         // 지점 간 이동할 때의 속도
    public float attackInterval = 0.2f;    // 이동하는 와중에 무작위 탄막을 뿌리는 주기 간격 (초)

    [Header("A4 Custom Shape Settings")]
    [Tooltip("Cross = + 형태, XShape = X 형태")]
    public A4AttackShape a4Shape = A4AttackShape.Cross; // 인스펙터에서 마우스로 딸깍 선택 가능!

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

        if (anim == null)
            anim = GetComponent<Animator>();

        sr = GetComponent<SpriteRenderer>();

        originalScale = transform.localScale;

        camMove = Camera.main.GetComponent<Cameramove>();
    }

    public void StartBoss()
    {
        if (isDead) return;
        if (isAttacking) return;

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
            if (p != null)
                player = p.transform;
        }
        return player;
    }

    public int burstCount = 5;

    // 기존 1, 2, 3번 공격에서 쓰이는 원래의 보스 몸 기준 발사 로직 (유지)
    void ThrowFromIndex(int index)
    {
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
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>() == null ? obj.GetComponent<Rigidbody2D>() : null;
            if (rb == null) rb = obj.GetComponent<Rigidbody2D>();
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

    public void ThrowRight()
    {
        ThrowFromIndex(1); // 오른쪽
    }

    public void ThrowLeft()
    {
        ThrowFromIndex(0); // 왼쪽
    }

    // ⭐ [4번 전용 수정] 지정된 지점에서 규칙적인 각도(+, X)로 정확히 4개 발사
    void A4ShapeThrowFromPosition(Vector3 spawnPosition)
    {
        if (throwPrefab == null) return;

        // 시작 기본 각도 설정
        // + 형태(Cross)는 0도(우), 90도(상), 180도(좌), 270도(하)
        // X 형태(XShape)는 대각선이므로 45도, 135도, 225度, 315도
        float startAngle = (a4Shape == A4AttackShape.Cross) ? 0f : 45f;

        // 90도씩 꺾으면서 정밀하게 딱 4개만 스폰하여 퍼트림
        for (int i = 0; i < 4; i++)
        {
            float targetAngle = startAngle + (i * 90f);

            GameObject obj = Instantiate(throwPrefab, spawnPosition, Quaternion.identity);
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            if (rb == null) continue;

            // 각도를 라디안 삼각함수 벡터 벡터 방향으로 변환
            Vector2 dir = new Vector2(
                Mathf.Cos(targetAngle * Mathf.Deg2Rad),
                Mathf.Sin(targetAngle * Mathf.Deg2Rad)
            ).normalized;

            // 속도는 기존 밸런스 데이터 유지
            float speed = Random.Range(minThrowSpeed, maxThrowSpeed);
            rb.velocity = dir * speed;
        }
    }

    Vector3 GetRandomDropPositionInCamera()
    {
        Camera cam = Camera.main;
        if (cam == null)
            return transform.position;

        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        Vector3 camPos = cam.transform.position;

        float minX = camPos.x - camWidth;
        float maxX = camPos.x + camWidth;

        float topY = camPos.y + camHeight;

        float spawnYOffset = 0.5f;

        float x = Random.Range(minX, maxX);
        float y = topY + spawnYOffset;

        return new Vector3(x, y, 0f);
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
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        transform.position = targetPos;
    }

    IEnumerator BossRoutine()
    {
        while (!isDead)
        {
            if (isAttacking)
            {
                yield return null;
                continue;
            }

            yield return new WaitForSeconds(attackDelay);

            int attackIndex = GetRandomAttack();
            Debug.Log("AttackIndex 선택됨: " + attackIndex);

            yield return StartCoroutine(DoAttack(attackIndex));
        }
    }

    int GetRandomAttack()
    {
        int index;

        do
        {
            index = Random.Range(1, 5);
        }
        while (index == lastAttackIndex);

        lastAttackIndex = index;
        return index;
    }

    IEnumerator DoAttack(int attackIndex)
    {
        isAttacking = true;
        Collider2D bossCollider = GetComponent<Collider2D>();

        if (attackIndex == 1)
        {
            if (defaultPos != null)
                transform.position = defaultPos.position;

            anim.SetInteger("AttackIndex", 1);
            anim.SetBool("IsAttacking", true);

            float startTime = Time.time;

            while (Time.time - startTime < 5f && !isDead)
            {
                yield return null;
            }

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

            if (camMove != null)
                StartCoroutine(camMove.ShakeCamera(5f, 0.15f));

            float startTime = Time.time;
            while (Time.time - startTime < 5f && !isDead)
                yield return null;

            StopCoroutine(dropCo);

            anim.SetBool("IsAttacking", false);
            anim.SetInteger("AttackIndex", 0);

            if (defaultPos != null)
                transform.position = defaultPos.position;

            if (bossCollider != null) bossCollider.enabled = true;

            isA2Active = false;
            isAttacking = false;
            yield break;
        }

        if (attackIndex == 3)
        {
            if (a1StartPos != null)
                transform.position = a1StartPos.position;

            anim.SetInteger("AttackIndex", 3);
            anim.SetBool("IsAttacking", true);

            if (a1EndPos != null)
            {
                while (Vector3.Distance(transform.position, a1EndPos.position) > 0.05f)
                {
                    transform.position = Vector3.MoveTowards(
                        transform.position,
                        a1EndPos.position,
                        moveSpeed * Time.deltaTime
                    );
                    yield return null;
                }

                transform.position = a1EndPos.position;
            }

            anim.SetBool("IsAttacking", false);
            anim.SetInteger("AttackIndex", 0);

            if (!isDead && defaultPos != null)
                transform.position = defaultPos.position;

            isAttacking = false;
            yield break;
        }

        // ============================================================
        // 🟢 A4 : 랜덤 좌표 대시 + '해당 랜덤 지점'에서 [정방향 4방향] 투사체 생성 패턴
        // ============================================================
        if (attackIndex == 4)
        {
            anim.SetInteger("AttackIndex", 1);
            anim.SetBool("IsAttacking", true);

            float patternStartTime = Time.time;
            float lastAttackTime = 0f;
            int currentTargetPointIndex = Random.Range(0, a4SpawnPoints.Length);

            while (Time.time - patternStartTime < a4AttackDuration && !isDead)
            {
                if (a4SpawnPoints != null && a4SpawnPoints.Length > 0)
                {
                    Transform targetPoint = a4SpawnPoints[currentTargetPointIndex];

                    if (targetPoint != null)
                    {
                        // 1. 무작위 좌표로 보스 이동
                        transform.position = Vector3.MoveTowards(
                            transform.position,
                            targetPoint.position,
                            slideSpeed * Time.deltaTime
                        );

                        // 2. 이동 중 주기에 맞춰 4방향 모양 발사 호출
                        if (Time.time - lastAttackTime >= attackInterval)
                        {
                            lastAttackTime = Time.time;

                            // ⭐ 바뀐 4방향 탄형 함수 호출
                            A4ShapeThrowFromPosition(targetPoint.position);
                        }

                        // 3. 지점 도달 시 리타겟팅
                        if (Vector3.Distance(transform.position, targetPoint.position) <= 0.05f)
                        {
                            int nextIndex;
                            do
                            {
                                nextIndex = Random.Range(0, a4SpawnPoints.Length);
                            } while (nextIndex == currentTargetPointIndex && a4SpawnPoints.Length > 1);

                            currentTargetPointIndex = nextIndex;
                        }
                    }
                }
                yield return null;
            }

            anim.SetBool("IsAttacking", false);
            anim.SetInteger("AttackIndex", 0);

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
        if (cam == null || sr == null)
            return transform.position;

        float camTopY = cam.transform.position.y + cam.orthographicSize;
        float camCenterX = cam.transform.position.x;

        float spriteTopOffset = sr.bounds.max.y - transform.position.y;

        return new Vector3(
            camCenterX,
            camTopY - spriteTopOffset,
            transform.position.z
        );
    }

    // ================================
    // 데미지 처리
    // ================================
    public void TakeDamage(int damage)
    {
        if (GameOverManager.Instance != null && GameOverManager.Instance.isGameOverSequenceRunning) return;

        if (isDead || isA2Active) return;

        hp -= damage;

        if (!isHitBlinking)
            StartCoroutine(HitBlink());

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

    // ================================
    // 사망 처리
    // ================================
    void Die()
    {
        if (GameOverManager.Instance != null && GameOverManager.Instance.isGameOverSequenceRunning) return;

        isDead = true;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopBGM();
        }

        ClearRemainingProjectiles();

        Vector3 deathPos = transform.position;

        StopAllCoroutines();
        isAttacking = false;
        isA2Active = false;

        transform.position = deathPos;

        PlayerAction player = FindObjectOfType<PlayerAction>();
        if (player != null)
            player.LockControl();

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

        foreach (GameObject p in projectiles)
        {
            Destroy(p);
        }

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
            GameManager_L.Instance.currentLanguage == Language.EN
            && bossDeathDialogue_EN != null
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
            float remainingTime =
                (1f - state.normalizedTime) * state.length;

            yield return new WaitForSecondsRealtime(remainingTime);
        }

        if (keyPrefab != null)
        {
            Vector3 dropPos = transform.position;
            dropPos.y += keyDropYOffset;
            Instantiate(keyPrefab, dropPos, Quaternion.identity);
        }

        if (player != null)
            player.UnlockControl();

        Destroy(gameObject);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerAction pAction = collision.gameObject.GetComponent<PlayerAction>();
            if (pAction != null)
            {
                pAction.TakeDirectDamage();
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (isDead) return;

        if (other.CompareTag("Player"))
        {
            PlayerAction pAction = other.GetComponent<PlayerAction>();
            if (pAction != null)
            {
                pAction.TakeDirectDamage();
            }
        }
    }
}