using UnityEngine;

public partial class NPCContactDamage : MonoBehaviour
{
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ApplyDamage(collision.gameObject);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ApplyDamage(other.gameObject);
        }
    }

    void ApplyDamage(GameObject playerObj)
    {
        PlayerAction pAction = playerObj.GetComponent<PlayerAction>();
        if (pAction != null)
        {
            // 이전에 만든 넉백 없는 데미지 함수 호출
            pAction.TakeDirectDamage();
        }
    }
}