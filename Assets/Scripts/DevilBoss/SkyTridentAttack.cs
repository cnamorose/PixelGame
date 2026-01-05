using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyTridentAttack : MonoBehaviour
{
    [Header("References")]
    public GameObject tridentPrefab;
    public Transform skyPointsParent;

    [Header("Attack Settings")]
    public float attackDuration = 6f;
    public float spawnInterval = 0.5f;

    Transform[] spawnPoints;
    List<GameObject> spawnedTridents = new List<GameObject>();

    void Awake()
    {
        int count = skyPointsParent.childCount;
        spawnPoints = new Transform[count];

        for (int i = 0; i < count; i++)
        {
            spawnPoints[i] = skyPointsParent.GetChild(i);
        }
    }

    public void StartSkyAttack()
    {
        StartCoroutine(SkyAttackRoutine());
    }

    IEnumerator SkyAttackRoutine()
    {
        float elapsed = 0f;

        while (elapsed < attackDuration)
        {
            SpawnOne();
            yield return new WaitForSeconds(spawnInterval);
            elapsed += spawnInterval;
        }

        ClearTridents();
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
        {
            if (t != null)
                Destroy(t);
        }
        spawnedTridents.Clear();
    }
}