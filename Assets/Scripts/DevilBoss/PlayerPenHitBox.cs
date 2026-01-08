using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPenHitBox : MonoBehaviour
{
    Collider2D col;
    HashSet<DevilHealth> hitThisSwing = new HashSet<DevilHealth>();

    void Awake()
    {
        col = GetComponent<Collider2D>();
        col.enabled = false;
    }

    public void EnableHitBox(bool enable)
    {
        col.enabled = enable;
        if (enable)
            hitThisSwing.Clear();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        DevilHealth devil = other.GetComponentInParent<DevilHealth>();
        if (devil != null && !hitThisSwing.Contains(devil))
        {
            hitThisSwing.Add(devil);
            devil.TakeDamage(10);

            PlayerAction player = GetComponentInParent<PlayerAction>();
            if (player != null)
                player.ApplyAttackRecoil();
        }
    }
}