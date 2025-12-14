using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person1NPC : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite[] crySprites;
    public Sprite[] walkSprites;

    [Header("Animation")]
    public float animInterval = 0.2f;

    [Header("Move")]
    public float moveSpeed = 2f;
    public float stopDistance = 0.1f;

    SpriteRenderer sr;
    Rigidbody2D rb;
    Coroutine animCoroutine;
    int spriteIndex = 0;
    bool isFreed = false;

    Vector3 originalScale;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        originalScale = transform.localScale;

        // ✅ 중력 ON
        rb.gravityScale = 3f;
        rb.freezeRotation = true;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        StartCry();
    }

    /* =========================
       울고 있기
       ========================= */
    void StartCry()
    {
        StopAnim();
        transform.localScale = originalScale;
        sr.flipX = false;

        animCoroutine = StartCoroutine(PlayAnimation(crySprites));
    }

    /* =========================
       외부 호출
       ========================= */
    public IEnumerator FreeAndWait(float targetX)
    {
        if (isFreed) yield break;
        isFreed = true;

        StopAnim();
        yield return StartCoroutine(WalkToX(targetX));
    }

    /* =========================
       걷기 (X축만 이동, Y는 중력)
       ========================= */
    IEnumerator WalkToX(float targetX)
    {
        rb.velocity = Vector2.zero;
        transform.localScale = originalScale;
        sr.flipX = false; // 항상 왼쪽

        animCoroutine = StartCoroutine(PlayAnimation(walkSprites));

        while (Mathf.Abs(transform.position.x - targetX) > stopDistance)
        {
            float dir = Mathf.Sign(targetX - transform.position.x);

            rb.MovePosition(
                new Vector2(
                    rb.position.x + dir * moveSpeed * Time.fixedDeltaTime,
                    rb.position.y   // ⭐ Y는 건드리지 않는다
                )
            );

            yield return new WaitForFixedUpdate();
        }

        rb.velocity = Vector2.zero;
        StopAnim();
        sr.sprite = walkSprites[0];
    }

    /* =========================
       공통
       ========================= */
    void StopAnim()
    {
        if (animCoroutine != null)
            StopCoroutine(animCoroutine);
    }

    IEnumerator PlayAnimation(Sprite[] sprites)
    {
        spriteIndex = 0;
        while (true)
        {
            sr.sprite = sprites[spriteIndex];
            spriteIndex = (spriteIndex + 1) % sprites.Length;
            yield return new WaitForSeconds(animInterval);
        }
    }
}