using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenProjectile : MonoBehaviour
{
    public int damage = 10;
    public float speed = 30f;
    public float lifeTime = 3f;

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

        // 물리 세팅 강제 (안 날아가는 문제 예방)
        rb.gravityScale = 0f;
        rb.drag = 0f;
        rb.angularDrag = 0f;
        rb.constraints = RigidbodyConstraints2D.None;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Fire(Vector2 dir)
    {
        if (rb == null) return;

        rb.velocity = dir.normalized * speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
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

        if (!other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}