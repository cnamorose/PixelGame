using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cameramove : MonoBehaviour
{
    public Transform target;               // 평소 따라갈 대상 (Player)
    public SpriteRenderer mapRenderer;     // 맵 경계용

    [Header("Clamp Settings")]
    public bool useClamp = true;
    public float minX, maxX, minY, maxY;

    [Header("Cutscene Settings")]
    public bool cutsceneMode = false;      // true면 플레이어 안 따라감
    public Vector3 cutsceneTarget;         // 컷신 동안 볼 위치
    public float cutsceneSpeed = 2f;

    Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();

        // target 자동 할당
        if (target == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null)
                target = p.transform;
        }

        CalculateLimits();
    }

    void LateUpdate()
    {
        // 🔴 컷신 중이면 플레이어 무시하고 cutsceneTarget으로만 이동
        if (cutsceneMode)
        {
            CalculateLimits();

            // 컷신 타겟을 카메라 가능 범위 안으로 Clamp
            float cx = Mathf.Clamp(cutsceneTarget.x, minX, maxX);
            float cy = Mathf.Clamp(cutsceneTarget.y, minY, maxY);

            Vector3 targetPos = new Vector3(cx, cy, transform.position.z);

            transform.position = Vector3.Lerp(
                transform.position,
                targetPos,
                Time.deltaTime * cutsceneSpeed
            );
            return;
        }

        // 평소에는 Player 따라가기
        if (target == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null)
                target = p.transform;
            else
                return;
        }

        CalculateLimits();

        float x = target.position.x;
        float y = target.position.y;

        if (useClamp)
        {
            x = Mathf.Clamp(x, minX, maxX);
            y = Mathf.Clamp(y, minY, maxY);
        }

        transform.position = new Vector3(x, y, transform.position.z);
    }

    void CalculateLimits()
    {
        if (mapRenderer == null || cam == null) return;

        Bounds bounds = mapRenderer.bounds;

        float camHeight = cam.orthographicSize * 2f;
        float camWidth = camHeight * cam.aspect;

        minX = bounds.min.x + camWidth / 2f;
        maxX = bounds.max.x - camWidth / 2f;

        minY = bounds.min.y + camHeight / 2f;
        maxY = bounds.max.y - camHeight / 2f;
    }

    // 🔵 컷신 시작할 때 호출
    public void StartCutscene(Vector3 worldPos)
    {
        cutsceneMode = true;
        cutsceneTarget = worldPos;
    }

    // 🟢 컷신 끝낼 때 호출 (다시 플레이어 따라가기)
    public void EndCutscene()
    {
        cutsceneMode = false;
    }
}
