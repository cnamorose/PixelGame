using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevilBossFadeIn : MonoBehaviour
{
    void Start()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartCoroutine(
                FadeIn(DialogueManager.Instance)
            );
        }
    }

    IEnumerator FadeIn(DialogueManager dm)
    {
        var fadeImage = dm.GetComponentInChildren<UnityEngine.UI.Image>();

        float t = 0f;
        float duration = 1f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(1f, 0f, t / duration);
            fadeImage.color = new Color(0, 0, 0, a);
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, 0);
    }
}