using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentItem : MonoBehaviour
{
    public string partName;
    public GameObject uiReal;

    [Header("효과음")]
    public AudioClip itemSFX;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        CollectPart();
    }

    void CollectPart()
    {
        // 이미 먹은 부품이면 무시
        if (GameManager_KM.Instance.HasPart(partName))
            return;

        // UI 표시
        if (uiReal != null)
            uiReal.SetActive(true);

        // 부품 등록
        GameManager_KM.Instance.AddPart(partName);

        // 효과음
        if (AudioManager.Instance != null && itemSFX != null)
            AudioManager.Instance.PlaySFX(itemSFX);

        Destroy(gameObject);
    }
}