using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyContactDamage : MonoBehaviour
{
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (!collision.collider.CompareTag("Player")) return;

    //    if (PlayerLifeManager.Instance != null)
    //    {
    //        PlayerLifeManager.Instance.LoseLife();
    //    }
    //}
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("닿음: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("플레이어 맞음!");
            PlayerLifeManager.Instance.LoseLife();
        }
    }
}
