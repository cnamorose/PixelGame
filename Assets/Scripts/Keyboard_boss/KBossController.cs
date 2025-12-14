using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KBossController : MonoBehaviour
{
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
    public int maxHp = 20;
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
    public float dropInterval = 0.4f;   
    public float dropHeight = 6f;        
    public float dropRangeX = 6f;

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

    void ThrowFromIndex(int index)
    {
        if (throwPrefab == null || throwPoints == null)
            return;

        if (index < 0 || index >= throwPoints.Length)
            return;

        Transform point = throwPoints[index];
        if (point == null) return;

        // ⭐ A1은 한 번에 1개만 던짐
        int throwCount = Random.Range(1, 3); // 1~2개 (원하면 1로 고정해도 됨)

        for (int i = 0; i < throwCount; i++)
        {
            GameObject obj = Instantiate(throwPrefab, point.position, Quaternion.identity);
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            if (rb == null) continue;

            // ⭐ 완전 랜덤 방향 (360도)
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

    Vector3 GetRandomDropPositionInCamera()
    {
        Camera cam = Camera.main;
        if (cam == null)
            return transform.position;

        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        Vector3 camPos = cam.transform.position;

        // 화면 좌우 범위
        float minX = camPos.x - camWidth;
        float maxX = camPos.x + camWidth;

        // 화면 최상단
        float topY = camPos.y + camHeight;

        // 약간 위에서 생성 (연출용)
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

    // ================================
    // 메인 보스 루프
    // ================================
    IEnumerator BossRoutine()
    {
        while (!isDead)
        {
            // 공격 중이면 대기
            if (isAttacking)
            {
                yield return null;
                continue;
            }

            yield return new WaitForSeconds(attackDelay);

            int attackIndex = GetRandomAttack();
            Debug.Log("AttackIndex 선택됨: " + attackIndex);

            // ⭐ 공격 실행은 DoAttack에게 위임
            yield return StartCoroutine(DoAttack(attackIndex));
        }
    }

    // ================================
    // 랜덤 공격 선택 (연속 방지)
    // ================================
    int GetRandomAttack()
    {
        int index;

        do
        {
            index = Random.Range(1, 4); // A1~A3
        }
        while (index == lastAttackIndex);

        lastAttackIndex = index;
        return index;
    }

    IEnumerator DoAttack(int attackIndex)
    {
        isAttacking = true;

        // =====================
        // A1 : 기존 자리에서 5초 액션
        // =====================
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


        // =====================
        // A2 : 지정 위치 + 3배 크기 + 5초 반복
        // =====================
        // A2 : 지정 위치 + 5초 반복
        if (attackIndex == 2)
        {
            isA2Active = true;
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

            isA2Active = false;
            isAttacking = false;
            yield break;
        }

        // =====================
        // A3 : 이동하면서 액션 → 끝에서 순간이동
        // =====================
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

            // ⭐ 즉시 원래 위치로 순간이동
            if (!isDead && defaultPos != null)
                transform.position = defaultPos.position;

            isAttacking = false;
            yield break;
        }
    }

    Vector3 GetA2CameraTopPosition()
    {
        Camera cam = Camera.main;
        if (cam == null || sr == null)
            return transform.position;

        // 카메라 기준
        float camTopY = cam.transform.position.y + cam.orthographicSize;
        float camCenterX = cam.transform.position.x;

        // 스프라이트 윗면 오프셋
        float spriteTopOffset = sr.bounds.max.y - transform.position.y;

        return new Vector3(
            camCenterX,                // X는 무조건 카메라 중심
            camTopY - spriteTopOffset, // 윗면이 천장에 닿도록
            transform.position.z
        );
    }



    // ================================
    // 데미지 처리
    // ================================
    public void TakeDamage(int damage)
    {
        if (isDead) return;

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
        isDead = true;

        Vector3 deathPos = transform.position;

        // 모든 행동 중지
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

    [SerializeField] float keyDropYOffset = -0.5f;

    IEnumerator DestroyAfterDeath()
    {
        PlayerAction player = FindObjectOfType<PlayerAction>();

        float waitTime = deathClip != null ? deathClip.length : 0.8f;
        yield return new WaitForSeconds(waitTime);

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

}
