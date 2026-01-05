using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingBallAttack : MonoBehaviour
{
    [Header("References")]
    public GameObject ballPrefab;
    public Transform[] spawnPoints;

    [Header("Attack Settings")]
    public float attackDuration = 10f;

    List<GameObject> spawnedBalls = new List<GameObject>();

    // 외부에서 호출
    public void StartBouncingAttack()
    {
        StartCoroutine(BouncingRoutine());
    }

    IEnumerator BouncingRoutine()
    {
        // 동시 생성
        foreach (Transform point in spawnPoints)
        {
            GameObject ball =
                Instantiate(ballPrefab, point.position, Quaternion.identity);

            spawnedBalls.Add(ball);
        }

        // 공격 지속
        yield return new WaitForSeconds(attackDuration);

        // 공격 종료 → 전부 제거
        ClearBalls();
    }

    void ClearBalls()
    {
        foreach (GameObject ball in spawnedBalls)
        {
            if (ball != null)
                Destroy(ball);
        }

        spawnedBalls.Clear();
    }
}