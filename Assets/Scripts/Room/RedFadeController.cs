using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RedFadeController : MonoBehaviour
{
    [SerializeField] Image fadeImage;

    public IEnumerator Play(
        float fadeIn = 0.8f,
        float hold = 1.2f,
        float fadeOut = 0.8f,
        float alpha = 0.6f
    )
    {
        fadeImage.gameObject.SetActive(true);

        Color c = new Color(1f, 0f, 0f, 0f);
        fadeImage.color = c;

        float t = 0f;
        while (t < fadeIn)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, alpha, t / fadeIn);
            fadeImage.color = c;
            yield return null;
        }

        yield return new WaitForSeconds(hold);

        t = 0f;
        while (t < fadeOut)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(alpha, 0f, t / fadeOut);
            fadeImage.color = c;
            yield return null;
        }

        fadeImage.gameObject.SetActive(false);
    }
}
