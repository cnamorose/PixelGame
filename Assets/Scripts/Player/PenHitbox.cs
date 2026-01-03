using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenHitbox : MonoBehaviour
{
    public int damage = 1;
    public float lifeTime = 0.15f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Monster monster = other.GetComponentInParent<Monster>();
        if (monster != null)
        {
            Vector2 dir =
                (monster.transform.position - transform.position).normalized;

            monster.TakeDamage(damage, dir);
            Destroy(gameObject);
        }
    }
}

