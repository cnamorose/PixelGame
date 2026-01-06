using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingBallAttack : MonoBehaviour, IDevilAttack
{
    [Header("References")]
    public GameObject ballPrefab;
    public Transform[] spawnPoints;

    List<GameObject> spawnedBalls = new();
    bool isRunning = false;

    // 공격 시작 (컨트롤러가 호출)
    public void StartAttack()
    {
        if (isRunning) return;
        isRunning = true;

        foreach (Transform point in spawnPoints)
        {
            GameObject ball =
                Instantiate(ballPrefab, point.position, Quaternion.identity);
            spawnedBalls.Add(ball);
        }
    }

    // 공격 종료 (컨트롤러가 호출)
    public void EndAttack()
    {
        if (!isRunning) return;
        isRunning = false;

        ClearBalls();
    }

    void ClearBalls()
    {
        foreach (GameObject ball in spawnedBalls)
            if (ball != null) Destroy(ball);

        spawnedBalls.Clear();
    }
}