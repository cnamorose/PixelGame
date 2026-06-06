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
    // 첫 번째 줄바꿈 후 폰트 크기를 10으로 줄여서 줄바꿈을 한 번 더 하고, 다시 원상복구(</size>)
    "A Game By\n<size=10>\n</size><color=#87CEEB>Haerim Kim</color>",

    "Director &\nGame Design\n<size=10>\n</size><color=#87CEEB>Haerim Kim</color>",

    "Programming\n<size=10>\n</size><color=#87CEEB>Haerim Kim</color>",

    "Art & UI Design\n<size=10>\n</size><color=#87CEEB>Haerim Kim</color>",

    "Music & Sound\n<size=10>\n</size>Generated via\n<color=#FF8C00>Suno AI</color>",

    "Used Font\n<size=10>\n</size>'Mulmaru'\nby mushsooni",

    "Special Thanks\n<size=10>\n</size><color=#008A7B>MILab</color>",

    "Thank You\nFor Playing!"
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