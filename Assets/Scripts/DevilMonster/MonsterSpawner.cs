using System.Collections;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public Transform leftSpawn;
    public Transform rightSpawn;

    public GameObject monster1;
    public GameObject monster2;
    public GameObject monster3;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            float delay = Random.Range(0.5f, 2f);
            yield return new WaitForSeconds(delay);

            SpawnMonster();
        }
    }

    void SpawnMonster()
    {
        Transform spawnPoint = Random.value < 0.5f ? leftSpawn : rightSpawn;

        int rand = Random.Range(1, 4);
        GameObject prefab = rand == 1 ? monster1 :
                            rand == 2 ? monster2 : monster3;

        GameObject monster = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

        // ▶ 크기 보정 (그림 비슷해도 체급 차이 나게)
        if (rand == 1)
            monster.transform.localScale = Vector3.one * 3.0f;
        else if (rand == 2)
            monster.transform.localScale = Vector3.one * 4.0f;
        else
            monster.transform.localScale = Vector3.one * 6.0f;
    }
}
