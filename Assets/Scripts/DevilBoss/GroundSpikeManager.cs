using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSpikeManager : MonoBehaviour
{
    public static GroundSpikeManager Instance;

    [Header("References")]
    public GameObject spikePrefab;
    public Transform spikePointsParent;

    [Header("Wave Settings")]
    public float waveInterval = 0.08f;   // 포인트 간 웨이브 간격
    public float groupDelay = 0.6f;      // 짝수 → 홀수 사이 딜레이

    Transform[] spikePoints;

    void Awake()
    {
        Instance = this;

        int count = spikePointsParent.childCount;
        spikePoints = new Transform[count];

        for (int i = 0; i < count; i++)
        {
            spikePoints[i] = spikePointsParent.GetChild(i);
        }
    }

    // ================================
    // 외부에서 호출할 함수 (공격 1세트)
    // ================================
    public void StartEvenOddWave()
    {
        StartCoroutine(EvenOddWaveRoutine());
    }

    IEnumerator EvenOddWaveRoutine()
    {
        // 1️⃣ 짝수 포인트 웨이브
        yield return StartCoroutine(SpawnWaveByParity(0));

        yield return new WaitForSeconds(groupDelay);

        // 2️⃣ 홀수 포인트 웨이브
        yield return StartCoroutine(SpawnWaveByParity(1));
    }

    // ================================
    // 짝수 / 홀수 웨이브
    // parity: 0 = 짝수, 1 = 홀수
    // ================================
    IEnumerator SpawnWaveByParity(int parity)
    {
        for (int i = 0; i < spikePoints.Length; i++)
        {
            if (i % 2 != parity) continue;

            Instantiate(
                spikePrefab,
                spikePoints[i].position,
                Quaternion.identity
            );

            yield return new WaitForSeconds(waveInterval);
        }
    }
}