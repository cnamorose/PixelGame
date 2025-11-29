using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAuto : MonoBehaviour
{
    public Transform target;               // Player (자동으로 찾게 만들 예정)
    public SpriteRenderer mapRenderer;     // 배경 spriteRenderer

    float minX, maxX, minY, maxY;

    Camera cam;

    void Start()
    {
        cam = Camera.main;

        // 플레이어 없으면 자동으로 찾아서 연결
        if (target == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
                target = player.transform;
        }

        CalculateLimits(); // 시작 시 Clamp 계산
    }

    void LateUpdate()
    {
        if (target == null)
        {
            // 씬 전환 직후, 한 프레임 동안 플레이어를 못 찾을 수도 있음
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
                target = player.transform;
            else
                return; // 아직 플레이어 없음 → 그냥 대기
        }

        // 스크린 크기 변경 감지 → Clamp 재계산
        CalculateLimitsIfNeeded();

        float clampX = Mathf.Clamp(target.position.x, minX, maxX);
        float clampY = Mathf.Clamp(target.position.y, minY, maxY);

        transform.position = new Vector3(clampX, clampY, transform.position.z);
    }

    void CalculateLimitsIfNeeded()
    {
        CalculateLimits();
    }

    void CalculateLimits()
    {
        Bounds bounds = mapRenderer.bounds;

        float camHeight = cam.orthographicSize * 2f;
        float camWidth = camHeight * cam.aspect;

        minX = bounds.min.x + camWidth / 2f;
        maxX = bounds.max.x - camWidth / 2f;

        minY = bounds.min.y + camHeight / 2f;
        maxY = bounds.max.y - camHeight / 2f;
    }
}
