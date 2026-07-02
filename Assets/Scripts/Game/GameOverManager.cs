using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameOverManager : MonoBehaviour
{
    public bool isGameOverSequenceRunning = false;
    public static GameOverManager Instance;

    public bool fromGameOver = false;

    [Header("UI")]
    public Image fadePanel;
    public GameObject gameOverPanel;
    public TMP_Text extraGameOverText;

    [Header("Localized Random Mentions")]
    [Tooltip("한국어 게임오버 랜덤 문구들")]
    public string[] ko_GameOverTexts = new string[]
    {
        "올해 졸업할 수 있으려나?",
        "좀 더 분발해봐.",
        "이래서 논문 쓰겠어?"
    };

    [Tooltip("영어 게임오버 랜덤 문구들")]
    public string[] en_GameOverTexts = new string[]
    {
        "Will I be able to graduate\nthis year?",
        "You need to work\na bit harder.",
        "At this rate,\nno credits for you."
    };

    [Header("Audio")]
    public AudioClip gameOverBGM;

    // 🔒 [중복 방지 핵심 키] 직전에 나왔던 문구 번호를 기억할 변수 (초기값은 절대 겹치지 않을 -1)
    private int lastKoIndex = -1;
    private int lastEnIndex = -1;

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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Room")
        {
            isGameOverSequenceRunning = false;

            gameOverPanel.SetActive(false);
            extraGameOverText.gameObject.SetActive(false);
            fadePanel.gameObject.SetActive(false);

            if (EventSystem.current != null)
                EventSystem.current.enabled = true;
        }
    }

    public void ShowGameOver()
    {
        if (isGameOverSequenceRunning) return;

        if (SceneManager.GetActiveScene().name == "Room")
            return;

        isGameOverSequenceRunning = true;
        fromGameOver = true;

        PlayerAction player = FindObjectOfType<PlayerAction>();
        if (player != null) player.LockControl();

        if (EventSystem.current != null)
            EventSystem.current.enabled = false;

        // UI 정리
        GameObject lifeUI = GameObject.Find("LifeUI");
        if (lifeUI != null) lifeUI.SetActive(false);

        GameObject timerUI = GameObject.Find("TimerUI");
        if (timerUI != null) timerUI.SetActive(false);

        GameObject CpartsUI = GameObject.Find("CpartsUI");
        if (CpartsUI != null) CpartsUI.SetActive(false);

        // 랜덤 문구 중복 검사 후 세팅
        SetRandomGameOverText();

        StartCoroutine(GameOverSequence());
    }

    // ⭐ [수정 완료] 직전 인덱스와 비교해서 다를 때까지 다시 무작위로 뽑는 안전한 검증 로직
    private void SetRandomGameOverText()
    {
        if (extraGameOverText == null) return;

        bool isEnglishMode = (GameManager_L.Instance != null && GameManager_L.Instance.currentLanguage == Language.EN);

        if (isEnglishMode)
        {
            if (en_GameOverTexts != null && en_GameOverTexts.Length > 0)
            {
                int randomIndex = lastEnIndex;

                // 문구가 2개 이상이면 직전과 다른 번호가 나올 때까지 계속 굴림
                if (en_GameOverTexts.Length > 1)
                {
                    while (randomIndex == lastEnIndex)
                    {
                        randomIndex = Random.Range(0, en_GameOverTexts.Length);
                    }
                }
                else
                {
                    randomIndex = 0; // 등록된 문구가 1개뿐이면 그냥 0번 출력
                }

                lastEnIndex = randomIndex; // 현재 뽑힌 번호를 직전 번호로 저장
                extraGameOverText.text = en_GameOverTexts[randomIndex];
            }
            else
            {
                extraGameOverText.text = "Game Over";
            }
        }
        else
        {
            if (ko_GameOverTexts != null && ko_GameOverTexts.Length > 0)
            {
                int randomIndex = lastKoIndex;

                // 문구가 2개 이상이면 직전과 다른 번호가 나올 때까지 계속 굴림
                if (ko_GameOverTexts.Length > 1)
                {
                    while (randomIndex == lastKoIndex)
                    {
                        randomIndex = Random.Range(0, ko_GameOverTexts.Length);
                    }
                }
                else
                {
                    randomIndex = 0;
                }

                lastKoIndex = randomIndex; // 현재 뽑힌 번호를 직전 번호로 저장
                extraGameOverText.text = ko_GameOverTexts[randomIndex];
            }
            else
            {
                extraGameOverText.text = "게임 오버";
            }
        }
    }

    private IEnumerator GameOverSequence()
    {
        fadePanel.gameObject.SetActive(true);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayBGM(gameOverBGM);

        float fadeTime = 1f;
        Color c = fadePanel.color;

        // 페이드 인
        for (float t = 0; t < fadeTime; t += Time.unscaledDeltaTime)
        {
            float alpha = Mathf.Lerp(0f, 1f, t / fadeTime);
            fadePanel.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }

        fadePanel.color = new Color(c.r, c.g, c.b, 1f);

        yield return new WaitForSeconds(2f);

        gameOverPanel.SetActive(true);
        yield return new WaitForSeconds(2f);

        extraGameOverText.gameObject.SetActive(true);
        yield return new WaitForSeconds(5f);

        if (AudioManager.Instance != null)
            AudioManager.Instance.FadeOutThenLoadScene("Room", 2f);
    }
}