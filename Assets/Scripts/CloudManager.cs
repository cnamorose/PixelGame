using UnityEngine;

public class CloudManager : MonoBehaviour
{
    [Header("구름 프리팹 설정")]
    public GameObject[] cloudPrefabs; // 프리팹 3개 할당

    [Header("스폰 영역 설정 (BoxCollider2D 필수)")]
    public BoxCollider2D spawnArea;   // 영역을 지정할 BoxCollider2D 드래그앤드롭

    [Header("구름 속성 제어")]
    public float minSpeed = 0.5f;
    public float maxSpeed = 1.5f;
    public float minSize = 0.6f;
    public float maxSize = 1.4f;

    [Header("생성 주기")]
    public float minSpawnTime = 3f;
    public float maxSpawnTime = 7f;

    private float minXBound; // 구름이 파괴될 왼쪽 끝 좌표 (자동 계산)

    void Start()
    {
        if (spawnArea == null)
        {
            Debug.LogError("CloudManager: spawnArea(BoxCollider2D)가 지정되지 않았습니다!");
            return;
        }

        // 콜라이더의 왼쪽 끝(min.x)에서 구름의 대략적인 크기만큼 더 왼쪽을 파괴 지점으로 잡음
        minXBound = spawnArea.bounds.min.x - 2f;

        // 구름 생성 루프 시작
        StartCoroutine(SpawnCloudRoutine());
    }

    System.Collections.IEnumerator SpawnCloudRoutine()
    {
        while (true)
        {
            float delay = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(delay);

            SpawnCloud();
        }
    }

    void SpawnCloud()
    {
        if (cloudPrefabs.Length == 0 || spawnArea == null) return;

        // 1. 프리팹 3개 중 하나 랜덤 선택
        int randomIndex = Random.Range(0, cloudPrefabs.Length);
        GameObject selectedPrefab = cloudPrefabs[randomIndex];

        // 2. BoxCollider2D 영역 내의 우측 경계선(max.x)에서 랜덤한 Y축 높이 계산
        Bounds bounds = spawnArea.bounds;
        float spawnX = bounds.max.x;
        float randomY = Random.Range(bounds.min.y, bounds.max.y);

        Vector3 spawnPosition = new Vector3(spawnX, randomY, 0f);

        // 3. 구름 생성
        GameObject cloudGo = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);

        // 4. 생성된 구름에 랜덤 속도 및 크기 주입
        MovingCloud cloudScript = cloudGo.GetComponent<MovingCloud>();
        if (cloudScript == null)
        {
            cloudScript = cloudGo.AddComponent<MovingCloud>();
        }

        float randomSpeed = Random.Range(minSpeed, maxSpeed);
        // 계산된 왼쪽 파괴 경계값(minXBound)을 넘겨줌
        cloudScript.Initialize(randomSpeed, minXBound, minSize, maxSize);
    }
}