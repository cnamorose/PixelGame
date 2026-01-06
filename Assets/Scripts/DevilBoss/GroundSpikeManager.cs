using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSpikeManager : MonoBehaviour, IDevilAttack
{
    public static GroundSpikeManager Instance;

    [Header("References")]
    public GameObject spikePrefab;
    public Transform spikePointsParent;

    [Header("Wave Settings")]
    public float waveInterval = 0.08f;
    public float groupDelay = 0.6f;

    Transform[] spikePoints;
    Coroutine runningRoutine;

    void Awake()
    {
        Instance = this;

        int count = spikePointsParent.childCount;
        spikePoints = new Transform[count];

        for (int i = 0; i < count; i++)
            spikePoints[i] = spikePointsParent.GetChild(i);
    }

    // 공격 시작 (컨트롤러가 호출)
    public void StartAttack()
    {
        if (runningRoutine != null) return;
        runningRoutine = StartCoroutine(WaveLoop());
    }

    // 공격 종료 (컨트롤러가 호출)
    public void EndAttack()
    {
        if (runningRoutine != null)
        {
            StopCoroutine(runningRoutine);
            runningRoutine = null;
        }
    }

    // 무한 루프 (시간 관리는 컨트롤러)
    IEnumerator WaveLoop()
    {
        while (true)
        {
            yield return StartCoroutine(SpawnWaveByParity(0));
            yield return new WaitForSeconds(groupDelay);
            yield return StartCoroutine(SpawnWaveByParity(1));
            yield return new WaitForSeconds(groupDelay);
        }
    }

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
