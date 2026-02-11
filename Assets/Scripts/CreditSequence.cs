using System.Collections;
using TMPro;
using UnityEngine;

public class CreditSequence : MonoBehaviour
{
    public TextMeshProUGUI creditText;

    public float fadeDuration = 1f;
    public float stayDuration = 2f;

    string[] credits =
    {
        "Director\nYour Name",
        "Programming\nYou",
        "Art\nYou",
        "Special Thanks\nEveryone",
        "Thank You\nFor Playing"
    };

    public void Play()
    {
        StartCoroutine(PlayCredits());
    }

    IEnumerator PlayCredits()
    {
        creditText.gameObject.SetActive(true);

        foreach (string line in credits)
        {
            creditText.text = line;
            yield return StartCoroutine(FadeIn());
            yield return new WaitForSeconds(stayDuration);
            yield return StartCoroutine(FadeOut());
        }

        creditText.gameObject.SetActive(false);
    }

    IEnumerator FadeIn()
    {
        float t = 0f;
        creditText.alpha = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            creditText.alpha = t / fadeDuration;
            yield return null;
        }
    }

    IEnumerator FadeOut()
    {
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            creditText.alpha = 1f - (t / fadeDuration);
            yield return null;
        }
    }
}