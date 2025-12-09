using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    public Transform respawnPoint; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 1) 생명 감소
            PlayerLifeManager.Instance.LoseLife();

            // 2) 생명이 남아 있다면 리스폰
            if (PlayerLifeManager.Instance.currentLife > 0)
            {
                other.transform.position = respawnPoint.position;
            }

        }
    }
}