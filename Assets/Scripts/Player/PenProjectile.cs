using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenProjectile : MonoBehaviour
{
    public int damage = 10;
    public float speed = 20f;

    [Header("사거리 조절")]
    // 💡 이 값을 유니티 인스펙터에서 줄이면 사거리가 짧아집니다!
    // (예: speed 30일 때 lifeTime 0.5면 약 15거리만큼 날아감)
    public float lifeTime = 0.3f;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            rb = GetComponentInChildren<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("PenProjectile에 Rigidbody2D 없음");
            return;
        }

        rb.gravityScale = 0f;
        rb.drag = 0f;
        rb.angularDrag = 0f;
        rb.constraints = RigidbodyConstraints2D.None;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    void Start()
    {
        // 💡 지정된 수명(lifeTime)이 지나면 무조건 파괴!
        Destroy(gameObject, lifeTime);
    }

    public void Fire(Vector2 dir)
    {
        if (rb == null) return;
        rb.velocity = dir.normalized * speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.gameObject.name.Contains("Player"))
        {
            return;
        }

        if (other.isTrigger)
        {
            return;
        }

        KBossController boss = other.GetComponent<KBossController>();
        if (boss == null)
            boss = other.GetComponentInParent<KBossController>();

        if (boss != null)
        {
            Debug.Log("보스 피격!");
            boss.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // 벽이나 바닥에 닿았을 때 파괴
        Destroy(gameObject);
    }
}