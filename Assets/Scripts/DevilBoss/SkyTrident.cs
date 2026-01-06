using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyTrident : MonoBehaviour
{
    public float warningDropDistance = 0.8f;
    public float warningTime = 0.6f;
    public float fallSpeed = 18f;

    Rigidbody2D rb;
    Collider2D col;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        rb.gravityScale = 0f;              // 중력 OFF
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    public void StartDrop()
    {
        StartCoroutine(DropRoutine());
    }

    IEnumerator DropRoutine()
    {
        Vector3 startPos = transform.position;
        Vector3 warningPos = startPos + Vector3.down * warningDropDistance;

        // 🔔 예고 이동 (이건 Transform OK)
        float t = 0f;
        while (t < 1f)
        {
            transform.position = Vector3.Lerp(startPos, warningPos, t);
            t += Time.deltaTime * 2f;
            yield return null;
        }

        transform.position = warningPos;
        yield return new WaitForSeconds(warningTime);

        // ⬇️ 낙하 시작 (여기서부터 물리)
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.velocity = Vector2.down * fallSpeed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            // 🛑 완전 정지
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;

            col.enabled = false; // 더 이상 충돌 안 함

            // 바닥 파묻힘 방지
            transform.position += Vector3.up * 0.01f;
        }
    }
}