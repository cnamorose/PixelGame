using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideTridentAttack : MonoBehaviour
{
    [Header("References")]
    public GameObject tridentPrefab;
    public Transform[] leftPoints;
    public Transform[] rightPoints;

    [Header("Attack Settings")]
    public float attackDuration = 4f;
    public float spawnInterval = 0.25f;

    [Header("Speed")]
    public float minSpeed = 10f;
    public float maxSpeed = 14f;

    // ============================
    // 외부에서 호출할 함수
    // ============================
    public void StartSideAttack()
    {
        StartCoroutine(SideAttackRoutine());
    }

    IEnumerator SideAttackRoutine()
    {
        float startTime = Time.time;

        while (Time.time - startTime < attackDuration)
        {
            SpawnRandomTrident();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    // ============================
    // 삼지창 하나 생성
    // ============================
    void SpawnRandomTrident()
    {
        bool spawnFromLeft = Random.value < 0.5f;

        Transform[] points = spawnFromLeft ? leftPoints : rightPoints;
        if (points.Length == 0) return;

        Transform spawnPoint =
            points[Random.Range(0, points.Length)];

        GameObject trident =
            Instantiate(tridentPrefab, spawnPoint.position, Quaternion.identity);

        Rigidbody2D rb = trident.GetComponent<Rigidbody2D>();
        SpriteRenderer sr = trident.GetComponent<SpriteRenderer>();

        float speed = Random.Range(minSpeed, maxSpeed);

        if (spawnFromLeft)
        {
            // 왼쪽 → 오른쪽
            rb.velocity = Vector2.right * speed;
            sr.flipX = false;
        }
        else
        {
            // 오른쪽 → 왼쪽
            rb.velocity = Vector2.left * speed;
            sr.flipX = true; // 좌우 반전
        }
    }
}
