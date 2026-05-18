using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageToPlayer : MonoBehaviour
{
    // 🔹 Trigger와 Collision 두 상황 모두 대응하게 만듭니다.
    private void OnTriggerEnter2D(Collider2D other) { HandleHit(other.gameObject); }
    private void OnCollisionEnter2D(Collision2D collision) { HandleHit(collision.gameObject); }

    void HandleHit(GameObject target)
    {
        if (!target.CompareTag("Player")) return;

        // 1. 데미지 입히기 (우리가 만든 넉백 없는 함수)
        PlayerAction pAction = target.GetComponent<PlayerAction>();
        if (pAction != null)
        {
            pAction.TakeDirectDamage();
        }
        else if (PlayerLifeManager.Instance != null)
        {
            PlayerLifeManager.Instance.LoseLife();
        }

        // 2. ✨ [핵심] 플레이어와 부딪혔으니 즉시 삭제!
        Destroy(gameObject);
    }
}