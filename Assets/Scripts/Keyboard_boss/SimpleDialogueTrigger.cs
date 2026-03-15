using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleDialogueTrigger : MonoBehaviour
{
    [Header("Dialogue Text")]
    [TextArea]
    public string messageKR;

    [TextArea]
    public string messageEN;

    [Header("SFX")]
    public AudioClip dialogueSFX;

    private bool triggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        bool isEN = GameManager_L.Instance.currentLanguage == Language.EN;

        string message =
            isEN && !string.IsNullOrEmpty(messageEN)
                ? messageEN
                : messageKR;

        DialogueManager.Instance.ShowSimpleDialogueAutoClose(message);
        if (dialogueSFX != null)
        {
            AudioManager.Instance.PlayOneShotSFX(dialogueSFX);
        }
    }
}