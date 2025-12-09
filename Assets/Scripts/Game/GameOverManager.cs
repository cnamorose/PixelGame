using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    [Header("UI")]
    public Image fadePanel;
    public GameObject gameOverPanel;
    public TMP_Text extraGameOverText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void ShowGameOver()
    {
        GameObject lifeUI = GameObject.Find("LifeUI");
        if (lifeUI != null) lifeUI.SetActive(false);

        GameObject timerUI = GameObject.Find("TimerUI");
        if (timerUI != null) timerUI.SetActive(false);

        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        fadePanel.gameObject.SetActive(true);

        float fadeTime = 1f;
        Color c = fadePanel.color;

        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(0f, 1f, t / fadeTime);
            fadePanel.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }

        fadePanel.color = new Color(c.r, c.g, c.b, 1f);

        yield return new WaitForSeconds(1f);

        gameOverPanel.SetActive(true);

        yield return new WaitForSeconds(1f);

        extraGameOverText.gameObject.SetActive(true);
    }
}