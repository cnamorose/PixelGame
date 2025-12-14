using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyItem : MonoBehaviour
{
    bool isAttached = false;
    Vector3 baseLocalPos;

    public void OnAttachedToPlayer()
    {
        isAttached = true;
        baseLocalPos = transform.localPosition;
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * 4f) * 0.1f;

        if (isAttached)
        {
            transform.localPosition = baseLocalPos + Vector3.up * offset;
        }
        else
        {
            // ⭐ 월드 상태에서는 그냥 현재 위치 기준으로 살짝만 흔들기
            transform.position += Vector3.up * Mathf.Sin(Time.time * 4f) * 0.001f;
        }
    }
}