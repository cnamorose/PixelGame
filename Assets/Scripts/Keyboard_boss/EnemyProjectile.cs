using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (PlayerLifeManager.Instance != null)
        {
            PlayerLifeManager.Instance.LoseLife();
        }

        Destroy(gameObject);
    }
}
