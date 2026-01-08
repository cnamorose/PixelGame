using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPenHitBox : MonoBehaviour
{
    Collider2D col;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        col.enabled = false; // ±âº»Àº ²¨µÒ
    }

    public void EnableHitBox(bool enable)
    {
        col.enabled = enable;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // º¸½º(¾Ç¸¶) ¸ÂÃß±â
        DevilHealth devil = other.GetComponentInParent<DevilHealth>();
        if (devil != null)
        {
            devil.TakeDamage(1);
        }
    }
}
