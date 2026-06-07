using UnityEngine;

public class MovingCloud : MonoBehaviour
{
    private float speed;
    private float minXBound; // 화면 왼쪽 끝 경계 (여기를 넘어가면 삭제)
    private float floatSpeed;
    private float floatRange;
    private float startY;
    private float randomOffset;

    // 매니저가 구름을 생성할 때 초기 속도와 경계값을 넘겨줄 메서드
    public void Initialize(float moveSpeed, float minX, float sizeMin, float sizeMax)
    {
        speed = moveSpeed;
        minXBound = minX;
        startY = transform.position.y;
        randomOffset = Random.Range(0f, 100f);

        // 둥둥거리는 효과를 위한 약간의 개인차 부여
        floatSpeed = Random.Range(0.5f, 1.5f);
        floatRange = Random.Range(0.1f, 0.3f);

        // 랜덤 크기 설정 (X, Y 비율 유지)
        float randomScale = Random.Range(sizeMin, sizeMax);
        transform.localScale = new Vector3(randomScale, randomScale, 1f);
    }

    void Update()
    {
        // 1. 왼쪽으로 이동
        transform.Translate(Vector3.left * speed * Time.deltaTime, Space.World);

        // 2. 이동하면서 위아래로 살짝 둥둥 떠다니는 효과 (기존 Sin 활용)
        float newY = startY + Mathf.Sin((Time.time + randomOffset) * floatSpeed) * floatRange;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // 3. 지정한 왼쪽 영역 밖으로 벗어나면 오브젝트 파괴
        if (transform.position.x < minXBound)
        {
            Destroy(gameObject);
        }
    }
}