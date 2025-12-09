using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public Transform respawnPoint;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void RespawnPlayer()
    {
        // ¼ø°£ÀÌµ¿
        transform.position = respawnPoint.position;

        // ±ôºıÀÓ È¿°ú ½ÃÀÛ
        StartCoroutine(BlinkEffect());
    }

    private IEnumerator BlinkEffect()
    {
        // 2¹ø ±ôºıÀÓ (Off¡æOn 2È¸)
        for (int i = 0; i < 2; i++)
        {
            spriteRenderer.enabled = false;  // ¼û±è
            yield return new WaitForSeconds(0.1f);

            spriteRenderer.enabled = true;   // º¸ÀÓ
            yield return new WaitForSeconds(0.1f);
        }
    }
}