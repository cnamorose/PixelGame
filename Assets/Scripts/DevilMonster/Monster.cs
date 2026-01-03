using System.Collections;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("Stats")]
    public int maxHp = 1;
    public float moveSpeed = 1.5f;
    public float knockbackForce = 6f;

    int currentHp;
    int moveDir; // -1 = 왼쪽, 1 = 오른쪽

    Rigidbody2D rigid;
    SpriteRenderer sr;
    Animator anim;
    Transform player;

    bool isKnockback = false;
    bool isDead = false;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        currentHp = maxHp;
        player = GameObject.FindWithTag("Player")?.transform;

        // ▶ 스폰 시 방향 결정 (핵심)
        if (player != null)
        {
            moveDir = player.position.x > transform.position.x ? 1 : -1;
            sr.flipX = moveDir < 0;
        }
        else
        {
            moveDir = -1;
        }
    }

    void Update()
    {
        if (isDead || isKnockback)
            return;

        rigid.velocity = new Vector2(moveDir * moveSpeed, 0);
    }

    // =====================
    // ▶ 데미지 처리
    // =====================
    public void TakeDamage(int damage, Vector2 hitDir)
    {
        if (isDead)
            return;

        currentHp -= damage;

        if (currentHp <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(Knockback(hitDir));
        }
    }

    IEnumerator Knockback(Vector2 hitDir)
    {
        isKnockback = true;
        rigid.velocity = Vector2.zero;

        if (anim != null)
            anim.speed = 0f;

        // ▶ 뒤로 밀기
        rigid.AddForce(hitDir * knockbackForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.25f);

        if (anim != null)
            anim.speed = 1f;

        isKnockback = false;
    }

    // =====================
    // ▶ 사망 처리
    // =====================
    void Die()
    {
        isDead = true;
        rigid.velocity = Vector2.zero;

        // ⭐ 보스 HP 감소
        DevilLifeBarController boss =
            FindObjectOfType<DevilLifeBarController>();

        if (boss != null)
        {
            boss.ReduceHP(10); // 몬스터 1마리 = 보스 HP 1 감소
        }

        StartCoroutine(FadeAndDestroy());
    }

    IEnumerator FadeAndDestroy()
    {
        float fadeTime = 0.4f;
        float t = 0f;
        Color c = sr.color;

        while (t < fadeTime)
        {
            c.a = Mathf.Lerp(1f, 0f, t / fadeTime);
            sr.color = c;
            t += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    // =====================
    // ▶ 플레이어 충돌
    // =====================
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead)
            return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerLifeManager.Instance.LoseLife();
            Destroy(gameObject);
        }
    }
}
