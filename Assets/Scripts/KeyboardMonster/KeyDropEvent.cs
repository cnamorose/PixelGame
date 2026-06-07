using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyDropEvent : MonoBehaviour
{
    public GameObject disappearTilemap;
    public Rigidbody2D keyRigidbody;

    public float shakeDuration = 0.4f;
    public float shakeAmount = 0.05f;
    public float freezeDuration = 3.0f; // 플레이어가 못 움직일 시간 (3초)

    private bool triggered = false;
    private Vector3 originalPos;
    private bool keyShown = false;

    [Header("SFX")]
    public AudioClip shakeSFX;


    void Start()
    {
        originalPos = disappearTilemap.transform.position;

        if (keyRigidbody != null)
        {
            keyRigidbody.gameObject.SetActive(false);
            keyRigidbody.gravityScale = 0f;
        }
    }

    void Update()
    {
        if (keyShown) return;

        if (GameManager_KM.Instance != null &&
            GameManager_KM.Instance.HasAllParts())
        {
            keyShown = true;

            keyRigidbody.gameObject.SetActive(true);
            keyRigidbody.gravityScale = 0f;
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("TRIGGER ENTER: " + other.name);

        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        if (!GameManager_KM.Instance.HasAllParts())
        {
            Debug.Log("부품이 아직 부족합니다!");
            return;
        }

        triggered = true;

        // 플레이어에게 붙어있는 진짜 스크립트인 'PlayerAction'을 찾아옵니다.
        PlayerAction playerAction = other.GetComponent<PlayerAction>();

        StartCoroutine(ShakeAndDisappear(playerAction));
    }

    IEnumerator ShakeAndDisappear(PlayerAction player)
    {
        // [1] 연출 시작: PlayerAction에 이미 만들어두신 LockControl 호출!
        if (player != null) player.LockControl();

        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeAmount;
            disappearTilemap.transform.position =
                originalPos + new Vector3(x, 0, 0);

            float dynamicPitch = Mathf.Lerp(1f, 1.3f, elapsed / shakeDuration);

            AudioManager.Instance.PlayOneShotWithPitch(
                shakeSFX,
                1.6f,
                dynamicPitch
            );

            elapsed += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        disappearTilemap.transform.position = originalPos;
        disappearTilemap.SetActive(false);

        // 열쇠 떨어지기 시작
        keyRigidbody.gravityScale = 1f;

        // [2] 열쇠가 툭 떨어지는 걸 감상하는 3초 대기 시간
        yield return new WaitForSeconds(freezeDuration);

        // [3] 연출 종료: 다시 움직일 수 있도록 UnlockControl 호출!
        if (player != null) player.UnlockControl();
    }
}