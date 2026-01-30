using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleDialogueTrigger : MonoBehaviour
{
    [TextArea]
    public string message;

    private bool triggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        DialogueManager.Instance.ShowSimpleDialogueAutoClose(message);
    }
}