using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class WordMovement : MonoBehaviour
{
    public float fallSpeed = 40f;
    public string word;

    RectTransform rt;
    RectTransform areaRT; // ⭐ TypingArea 기준

    private TypingGameManager manager;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        areaRT = transform.parent.GetComponent<RectTransform>();

        manager = FindObjectOfType<TypingGameManager>(); // ⭐ 추가
    }

    void Update()
    {
        rt.anchoredPosition += Vector2.down * fallSpeed * Time.deltaTime;

        // ⭐ TypingArea 맨 아래 기준
        float bottom = -areaRT.rect.height - rt.rect.height;

        if (rt.anchoredPosition.y < bottom)
        {
            if (manager != null)
                manager.OnWordMissed();  // ⭐ 놓침 카운트

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