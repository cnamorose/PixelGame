using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyTridentAttack : MonoBehaviour, IDevilAttack
{
    [Header("References")]
    public GameObject tridentPrefab;
    public Transform skyPointsParent;

    [Header("Attack Settings")]
    public float spawnInterval = 0.5f; // 간격만 유지

    Transform[] spawnPoints;
    List<GameObject> spawnedTridents = new();

    Coroutine runningRoutine;

    void Awake()
    {
        int count = skyPointsParent.childCount;
        spawnPoints = new Transform[count];

        for (int i = 0; i < count; i++)
            spawnPoints[i] = skyPointsParent.GetChild(i);
    }

    // 공격 시작 (컨트롤러가 호출)
    public void StartAttack()
    {
        runningRoutine = StartCoroutine(SkyAttackLoop());
    }

    // 공격 종료 (컨트롤러가 호출)
    public void EndAttack()
    {
        if (runningRoutine != null)
            StopCoroutine(runningRoutine);

        ClearTridents();
    }

    // 무한 루프 (언제 멈출지는 컨트롤러가 결정)
    IEnumerator SkyAttackLoop()
    {
        while (true)
        {
            SpawnOne();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnOne()
    {
        Transform point =
            spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject obj =
            Instantiate(tridentPrefab, point.position, Quaternion.identity);

        spawnedTridents.Add(obj);
        obj.GetComponent<SkyTrident>().StartDrop();
    }

    void ClearTridents()
    {
        foreach (var t in spawnedTridents)
            if (t != null) Destroy(t);

        spawnedTridents.Clear();
    }
}

