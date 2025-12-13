using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileStepDust : MonoBehaviour
{
    public ParticleSystem dustParticle;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player"))
            return;

        ContactPoint2D contact = collision.contacts[0];

        dustParticle.transform.position = contact.point + Vector2.down * 0.05f;

        dustParticle.Play();
    }
}
