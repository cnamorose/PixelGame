using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingBall : MonoBehaviour
{
    public float bounceForce = 16f;
    public LayerMask bounceLayers;

    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // 초기 방향 랜덤
        Vector2 dir = Random.insideUnitCircle.normalized;
        rb.AddForce(dir * bounceForce, ForceMode2D.Impulse);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Ground"))
            return;

        Debug.Log("바닥 충돌!");

        rb.velocity = Vector2.zero;

        // y는 무조건 양수
        float x = Random.Range(-1f, 1f);
        float y = Random.Range(0.7f, 1f);

        Vector2 dir = new Vector2(x, y).normalized;

        rb.AddForce(dir * 20f, ForceMode2D.Impulse);
    }
}