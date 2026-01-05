using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trident : MonoBehaviour
{
    public float lifeTime = 6f;     // 안전망
    public float margin = 0.1f;     // 화면 밖 여유

    Camera cam;

    void Start()
    {
        cam = Camera.main;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (cam == null) return;

        Vector3 viewportPos = cam.WorldToViewportPoint(transform.position);

  

        // 화면 밖이면 제거
        if (viewportPos.x < -margin || viewportPos.x > 1 + margin ||
            viewportPos.y < -margin || viewportPos.y > 1 + margin)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 플레이어 데미지
            Destroy(gameObject);
        }
    }
}