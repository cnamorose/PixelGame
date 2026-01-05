using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyTrident : MonoBehaviour
{
    public float warningDropDistance = 0.8f;
    public float warningTime = 0.6f;
    public float fallSpeed = 18f;

    bool isFalling = false;

    Rigidbody2D rb;
    Collider2D col;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    public void StartDrop()
    {
        StartCoroutine(DropRoutine());
    }

    IEnumerator DropRoutine()
    {
        Vector3 startPos = transform.position;
        Vector3 warningPos = startPos + Vector3.down * warningDropDistance;

        // 예고 이동
        float t = 0f;
        while (t < 1f)
        {
            transform.position = Vector3.Lerp(startPos, warningPos, t);
            t += Time.deltaTime * 2f;
            yield return null;
        }

        transform.position = warningPos;
        yield return new WaitForSeconds(warningTime);

        isFalling = true;
    }

    void Update()
    {
        if (!isFalling) return;

        transform.position += Vector3.down * fallSpeed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            // 🔥 완전 종료 세트
            isFalling = false;

            rb.velocity = Vector2.zero;
            rb.simulated = false;   // 물리 OFF
            col.enabled = false;    // ⭐ 충돌 OFF (이게 핵심)

            // 살짝 위로 스냅 (바닥 파묻힘 방지)
            transform.position += Vector3.up * 0.01f;
        }
    }
}