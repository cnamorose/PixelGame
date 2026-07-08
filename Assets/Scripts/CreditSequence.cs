using System.Collections;
using TMPro;
using UnityEngine;

public class CreditSequence : MonoBehaviour
{
    public TextMeshProUGUI creditText;

    public float fadeDuration = 1f;
    public float stayDuration = 2f;

    // ⭐ 다른 스크립트에서 감시할 수 있도록 public 선언
    [HideInInspector] public bool isEnded = false;

    string[] credits =
    {
        "A Game By\n<size=10>\n</size><color=#87CEEB>RiMimic</color>",
        "Director &\nGame Design\n<size=10>\n</size><color=#87CEEB>RiMimic</color>",
        "Programming\n<size=10>\n</size><color=#87CEEB>RiMimic</color>",
        "Art & UI Design\n<size=10>\n</size><color=#87CEEB>RiMimic</color>",
        "Music & Sound\n<size=10>\n</size>Generated via\n<color=#FF8C00>Suno AI</color>",
        "Used Font\n<size=10>\n</size>'Mulmaru'\nby mushsooni",
        "Special Thanks\n<size=10>\n</size><color=#008A7B>MILab</color>",
        "Thank You\nFor Playing!"
    };

    // ⭐ 매개변수 없는 깔끔한 원래의 Play 함수
    public void Play()
    {
        StartCoroutine(PlayCredits());
    }

    IEnumerator PlayCredits()
    {
        isEnded = false; // 시작할 때 false 고정

        creditText.gameObject.SetActive(true);

        foreach (string line in credits)
        {
            creditText.text = line;
            yield return StartCoroutine(FadeIn());
            yield return new WaitForSeconds(stayDuration);
            yield return StartCoroutine(FadeOut());
        }

        creditText.gameObject.SetActive(false);

        isEnded = true; // 완벽히 다 사라지면 진짜 끝(true)
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