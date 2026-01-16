using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunCameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;            

    [Header("Background")]
    public SpriteRenderer background;    

    [Header("Follow")]
    public float smoothSpeed = 5f;
    public float fixedY = 0f;            

    Camera cam;

    float minX;
    float maxX;

    void Start()
    {
        cam = GetComponent<Camera>();
        CalculateLimits();
    }

    void LateUpdate()
    {
        if (target == null || background == null) return;

        CalculateLimits();

        float targetX = Mathf.Clamp(
            target.position.x,
            minX,
            maxX
        );

        Vector3 desiredPos = new Vector3(
            targetX,
            fixedY,
            transform.position.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPos,
            Time.deltaTime * smoothSpeed
        );
    }

    void CalculateLimits()
    {
        if (cam == null || background == null) return;

        Bounds bounds = background.bounds;

        float camHeight = cam.orthographicSize * 2f;
        float camWidth = camHeight * cam.aspect;

        minX = bounds.min.x + camWidth / 2f;
        maxX = bounds.max.x - camWidth / 2f;
    }
}