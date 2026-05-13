using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems; // ⭐ 추가: 이벤트 시스템 제어용

public class GameOverManager : MonoBehaviour
{
    public bool isGameOverSequenceRunning = false;
    public static GameOverManager Instance;

    public bool fromGameOver = false;

    [Header("UI")]
    public Image fadePanel; // ⭐ 중요: 인스펙터에서 이 이미지의 'Raycast Target'이 체크되어 있어야 합니다.
    public GameObject gameOverPanel;
    public TMP_Text extraGameOverText;

    [Header("Audio")]
    public AudioClip gameOverBGM;

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

            // ⭐ 씬이 로드되면 다시 클릭과 조작이 가능하도록 복구
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

        // ⭐ 1. 플레이어 조작 즉시 잠금 (키보드 입력 차단)
        PlayerAction player = FindObjectOfType<PlayerAction>();
        if (player != null) player.LockControl();

        // ⭐ 2. UI 이벤트 시스템 비활성화 (마우스 클릭 차단)
        if (EventSystem.current != null)
            EventSystem.current.enabled = false;

        // UI 정리
        GameObject lifeUI = GameObject.Find("LifeUI");
        if (lifeUI != null) lifeUI.SetActive(false);

        GameObject timerUI = GameObject.Find("TimerUI");
        if (timerUI != null) timerUI.SetActive(false);

        GameObject CpartsUI = GameObject.Find("CpartsUI");
        if (CpartsUI != null) CpartsUI.SetActive(false);

        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        // ⭐ fadePanel을 켜는 순간 'Raycast Target'이 활성화되어 클릭을 모두 막습니다.
        fadePanel.gameObject.SetActive(true);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayBGM(gameOverBGM);

        float fadeTime = 1f;
        Color c = fadePanel.color;

        // 페이드 인
        for (float t = 0; t < fadeTime; t += Time.unscaledDeltaTime) // TimeScale 영향 안 받게 unscaled 권장
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

        // 마지막 룸 이동
        if (AudioManager.Instance != null)
            AudioManager.Instance.FadeOutThenLoadScene("Room", 2f);
    }
}