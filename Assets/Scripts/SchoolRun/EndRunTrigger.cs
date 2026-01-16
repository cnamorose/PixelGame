using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndRunTrigger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        DialogueManager.Instance.PlayCutsceneFadeOnly(
            DialogueManager.CutsceneType.DevilEnd
        );
    }
}