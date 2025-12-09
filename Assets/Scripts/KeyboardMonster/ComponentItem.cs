using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentItem : MonoBehaviour
{
    public string partName;
    public GameObject uiReal;   // 실제 컬러 UI만 필요

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CollectPart();
        }
    }

    void CollectPart()
    {
        if (uiReal != null) uiReal.SetActive(true);

        Destroy(gameObject); // 맵에 있던 아이템 제거
    }
}