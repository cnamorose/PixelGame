using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WordMovement : MonoBehaviour
{
    public float fallSpeed = 20f;
    public string word;

    RectTransform rt;
    RectTransform areaRT; // TypingArea 기준

    private TypingGameManager manager;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        areaRT = transform.parent.GetComponent<RectTransform>();

        manager = FindObjectOfType<TypingGameManager>();
    }

    void Update()
    {
        rt.anchoredPosition += Vector2.down * fallSpeed * Time.deltaTime;

        float bottom = -areaRT.rect.height - rt.rect.height;

        if (rt.anchoredPosition.y < bottom)
        {
            if (manager != null)
            {
                // ⭐ 수정: 매니저 호출 시 자기 자신(this)을 매개변수로 넘겨줍니다.
                manager.OnWordMissed(this);
            }

            Destroy(gameObject);
        }
    }

    public float GetY()
    {
        return rt.anchoredPosition.y;
    }

    public void SetWord(string w)
    {
        word = w;
        GetComponent<TextMeshProUGUI>().text = w;
    }
}