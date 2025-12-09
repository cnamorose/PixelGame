using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudFloat : MonoBehaviour
{
    public float range = 0.3f;        // 이동 범위
    public float speed = 0.5f;        // 전체 이동 속도

    Vector3 startPos;
    float offsetX;
    float offsetY;

    void Start()
    {
        startPos = transform.position;

        // 각 오브젝트마다 다른 패턴을 만들기 위한 랜덤 오프셋
        offsetX = Random.Range(0f, 10f);
        offsetY = Random.Range(0f, 10f);
    }

    void Update()
    {
        float x = Mathf.Sin((Time.time + offsetX) * speed) * range;
        float y = Mathf.Cos((Time.time + offsetY) * speed * 0.5f) * range;

        transform.position = startPos + new Vector3(x, y, 0);
    }
}
