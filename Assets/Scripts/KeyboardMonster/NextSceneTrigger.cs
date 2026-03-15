using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextSceneTrigger : MonoBehaviour
{
    public string nextSceneName;
    public SceneFadeController fadeController;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        if (AudioManager.Instance != null &&
            AudioManager.Instance.oneShotSFXSource != null)
        {
            AudioManager.Instance.oneShotSFXSource.Stop();
        }

        fadeController.FadeAndLoadScene(nextSceneName);
    }
}
