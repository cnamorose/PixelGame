using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person2NPC : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite[] coffeeSprites;   // 2
    public Sprite phoneSprite;       // 1
    public Sprite idleSprite;        // 1
    public Sprite[] walkSprites;

    [Header("Animation")]
    public float coffeeAnimInterval = 0.3f;
    public float phoneAnimInterval = 0.8f;

    [Header("State Time")]
    public float coffeeTime = 3f;
    public float phoneTime = 2f;

    [Header("Move")]
    public float moveSpeed = 2f;
    public float stopDistance = 0.1f;

    SpriteRenderer sr;
    Rigidbody2D rb;
    Coroutine animCoroutine;
    bool isFreed = false;

    Vector3 originalScale;
    int spriteIndex = 0;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        originalScale = transform.localScale;

        rb.gravityScale = 3f;
        rb.freezeRotation = true;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        StartCoroutine(StateRoutine());
    }

    IEnumerator StateRoutine()
    {
        while (true)
        {
            animCoroutine = StartCoroutine(
                PlayAnimation(coffeeSprites, coffeeAnimInterval)
            );
            yield return new WaitForSeconds(coffeeTime);
            StopAnim();

            sr.sprite = phoneSprite;
            yield return new WaitForSeconds(phoneTime);


            sr.sprite = idleSprite;
            yield return new WaitForSeconds(1f);
        }
    }


    public IEnumerator FreeAndWait(float targetX)
    {
        if (isFreed) yield break;
        isFreed = true;

        StopAllCoroutines();
        StopAnim();

        yield return StartCoroutine(WalkToX(targetX));
    }


    IEnumerator WalkToX(float targetX)
    {
        rb.velocity = Vector2.zero;
        transform.localScale = originalScale;
        sr.flipX = false; 

        animCoroutine = StartCoroutine(PlayAnimation(walkSprites, 0.2f));

        while (Mathf.Abs(transform.position.x - targetX) > stopDistance)
        {
            float dir = Mathf.Sign(targetX - transform.position.x);

            rb.MovePosition(
                new Vector2(
                    rb.position.x + dir * moveSpeed * Time.fixedDeltaTime,
                    rb.position.y
                )
            );

            yield return new WaitForFixedUpdate();
        }

        rb.velocity = Vector2.zero;
        StopAnim();
        sr.sprite = walkSprites[0];
    }


    void StopAnim()
    {
        if (animCoroutine != null)
            StopCoroutine(animCoroutine);
    }

    IEnumerator PlayAnimation(Sprite[] sprites, float interval)
    {
        spriteIndex = 0;
        while (true)
        {
            sr.sprite = sprites[spriteIndex];
            spriteIndex = (spriteIndex + 1) % sprites.Length;
            yield return new WaitForSeconds(interval);
        }
    }
}