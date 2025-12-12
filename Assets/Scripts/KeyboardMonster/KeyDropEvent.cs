using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyDropEvent : MonoBehaviour
{
    public GameObject disappearTilemap;   
    public Rigidbody2D keyRigidbody;    

    public float shakeDuration = 0.4f;
    public float shakeAmount = 0.05f;

    private bool triggered = false;
    private Vector3 originalPos;

    void Start()
    {
        originalPos = disappearTilemap.transform.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("TRIGGER ENTER: " + other.name);

        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        // 조건: 부품 
        if (!GameManager_KM.Instance.HasAllParts())
        {
            Debug.Log("부품이 아직 부족합니다!");
            return;
        }

        triggered = true;
        StartCoroutine(ShakeAndDisappear());
    }

    IEnumerator ShakeAndDisappear()
    {
        float elapsed = 0f;

        // 흔들림 연출
        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeAmount;
            disappearTilemap.transform.position =
                originalPos + new Vector3(x, 0, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        disappearTilemap.transform.position = originalPos;
        disappearTilemap.SetActive(false);

        keyRigidbody.gravityScale = 1f;
    }
}
