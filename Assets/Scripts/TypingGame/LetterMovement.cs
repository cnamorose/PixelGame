using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LetterMovement : MonoBehaviour
{
    public float fallSpeed = 50f;
    public char letterChar;

    RectTransform rt;
    RectTransform canvasRT;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        canvasRT = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }

    void Update()
    {
        rt.anchoredPosition += Vector2.down * fallSpeed * Time.deltaTime;

        // ⭐ 핵심: 실제 화면 기준 바닥
        float canvasBottom = -canvasRT.rect.height;

        // 글자가 화면 아래를 완전히 벗어났을 때
        if (rt.anchoredPosition.y < canvasBottom - rt.rect.height)
        {
            Destroy(gameObject);
        }
    }

    public float GetY()
    {
        return rt.anchoredPosition.y;
    }
}