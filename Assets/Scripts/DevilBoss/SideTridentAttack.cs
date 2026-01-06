using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideTridentAttack : MonoBehaviour, IDevilAttack
{
    [Header("References")]
    public GameObject tridentPrefab;
    public Transform[] leftPoints;
    public Transform[] rightPoints;

    [Header("Attack Settings")]
    public float spawnInterval = 0.25f;

    [Header("Speed")]
    public float minSpeed = 10f;
    public float maxSpeed = 14f;

    Coroutine runningRoutine;
    List<GameObject> spawnedTridents = new List<GameObject>();

    // 공격 시작
    public void StartAttack()
    {
        if (runningRoutine != null) return;
        runningRoutine = StartCoroutine(AttackLoop());
    }

    // 공격 종료
    public void EndAttack()
    {
        if (runningRoutine != null)
        {
            StopCoroutine(runningRoutine);
            runningRoutine = null;
        }

        // 이미 생성된 삼지창 전부 제거
        ClearTridents();
    }

    IEnumerator AttackLoop()
    {
        while (true)
        {
            SpawnRandomTrident();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnRandomTrident()
    {
        bool spawnFromLeft = Random.value < 0.5f;
        Transform[] points = spawnFromLeft ? leftPoints : rightPoints;
        if (points.Length == 0) return;

        Transform spawnPoint =
            points[Random.Range(0, points.Length)];

        GameObject trident =
            Instantiate(tridentPrefab, spawnPoint.position, Quaternion.identity);

        spawnedTridents.Add(trident); // 추적

        Rigidbody2D rb = trident.GetComponent<Rigidbody2D>();
        SpriteRenderer sr = trident.GetComponent<SpriteRenderer>();

        float speed = Random.Range(minSpeed, maxSpeed);

        if (spawnFromLeft)
        {
            rb.velocity = Vector2.right * speed;
            sr.flipX = false;
        }
        else
        {
            rb.velocity = Vector2.left * speed;
            sr.flipX = true;
        }
    }

    void ClearTridents()
    {
        foreach (var t in spawnedTridents)
        {
            if (t != null)
                Destroy(t);
        }

        spawnedTridents.Clear();
    }
}