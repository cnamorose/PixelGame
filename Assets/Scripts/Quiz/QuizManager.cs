using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static DialogueManager;

public class QuizManager : MonoBehaviour
{
    [Header("BGM")]
    public AudioClip quizBGM;

    [Header("Dialogue")]
    public DialogueSequence quizClearDialogue;
    public DialogueSequence quizClearDialogue_EN;

    [Header("Devil")]
    public GameObject devilObject;

    public PlayerLifeManager playerLife;

    private float timeLimit = 5f;
    private float currentTime;
    private bool isAnswering = false;
    public TMP_Text timerText;

    [Header("Quiz Data (Multi-Language)")]
    [Tooltip("비어있으면 Resources/QuizAnswer 폴더에서 자동 로드합니다.")]
    public QuizData[] quizList;       // 한국어 퀴즈 리스트

    [Tooltip("비어있으면 Resources/QuizAnswer_e 폴더에서 자동 로드합니다.")]
    public QuizData[] quizList_EN;    // 영어 퀴즈 리스트

    private List<QuizData> quizPool;  // 중복 방지용 실시간 문제 풀

    private QuizData currentQuiz;

    [Header("UI")]
    public TMP_Text questionText;
    public TMP_Text[] answerTexts;

    [Header("Fade UI")]
    public Image fadePanel;

    [Header("UI Block")]
    public GameObject questionPanel;

    public GameObject timerUI;


    private int quizCount = 0;
    private int maxQuizCount = 6;

    bool isQuizEnding = false;

    void Start()
    {
        // 1. 현재 설정된 언어에 맞는 배열 데이터 가져오기
        QuizData[] selectedQuizList = GetSelectedQuizList();

        // 2. 만약 인스펙터 창이 비어있다면(Length가 0이라면) Resources 폴더에서 자동 로드
        if (selectedQuizList == null || selectedQuizList.Length == 0)
        {
            selectedQuizList = LoadQuizzesFromResources();
        }

        // 3. 최종 확정된 퀴즈 리스트로 문제 풀 구성
        quizPool = new List<QuizData>(selectedQuizList);

        playerLife = FindObjectOfType<PlayerLifeManager>();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopAmbient();
            AudioManager.Instance.PlayBGM(quizBGM);
        }

        LoadRandomQuiz();
    }

    void Update()
    {
        if (isQuizEnding) return;
        if (!isAnswering) return;

        currentTime -= Time.deltaTime;

        timerText.text = Mathf.Ceil(currentTime).ToString();

        if (currentTime <= 0)
        {
            TimeOut();
        }
    }

    // 언어 설정에 따라 인스펙터 변수 할당 분기
    private QuizData[] GetSelectedQuizList()
    {
        bool isEN = GameManager_L.Instance != null && GameManager_L.Instance.currentLanguage == Language.EN;
        return isEN ? quizList_EN : quizList;
    }

    // 인스펙터가 비어있을 때 폴더 내부 에셋 자동 로드
    private QuizData[] LoadQuizzesFromResources()
    {
        bool isEN = GameManager_L.Instance != null && GameManager_L.Instance.currentLanguage == Language.EN;

        // Resources 폴더 하위의 경로를 지정합니다.
        string folderPath = isEN ? "QuizAnswer_e" : "QuizAnswer";

        // 해당 폴더 내의 모든 QuizData 타입의 ScriptableObject를 가져옵니다.
        QuizData[] loadedQuizzes = Resources.LoadAll<QuizData>(folderPath);

        if (loadedQuizzes == null || loadedQuizzes.Length == 0)
        {
            Debug.LogError($"[QuizManager] Resources/{folderPath} 폴더에 QuizData 파일이 존재하지 않습니다!");
        }

        return loadedQuizzes;
    }

    public void LoadRandomQuiz()
    {
        if (quizPool.Count == 0 || quizCount >= maxQuizCount)
        {
            ShowQuizClear();
            return;
        }

        int rand = UnityEngine.Random.Range(0, quizPool.Count);
        currentQuiz = quizPool[rand];
        quizPool.RemoveAt(rand);

        questionText.text = currentQuiz.question;

        for (int i = 0; i < 4; i++)
        {
            answerTexts[i].text = currentQuiz.answers[i];
        }

        currentTime = timeLimit;
        isAnswering = true;
    }

    public void CheckAnswer(int index)
    {
        if (isQuizEnding) return;

        if (index == currentQuiz.correctIndex)
        {
            isAnswering = false;
            quizCount++;
            LoadRandomQuiz();
        }
        else
        {
            playerLife.LoseLife();

            if (playerLife.currentLife > 0)
            {
                LoadRandomQuiz();
            }
        }
    }

    void TimeOut()
    {
        if (isQuizEnding) return;

        isAnswering = false;
        playerLife.LoseLife();

        if (playerLife.currentLife > 0)
        {
            LoadRandomQuiz();
        }
    }

    void ShowQuizClear()
    {
        if (isQuizEnding) return;

        isQuizEnding = true;
        isAnswering = false;

        if (questionPanel != null)
            questionPanel.SetActive(false);

        StartCoroutine(QuizClearSequence());
    }

    IEnumerator QuizClearSequence()
    {
        GameObject lifeUI = GameObject.Find("LifeUI");
        if (lifeUI != null) lifeUI.SetActive(false);

        if (devilObject != null)
            devilObject.SetActive(false);

        fadePanel.gameObject.SetActive(true);

        float fadeTime = 1f;
        Color c = fadePanel.color;

        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(0f, 1f, t / fadeTime);
            fadePanel.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }
        fadePanel.color = new Color(1f, 1f, 1f, 1f);

        yield return new WaitForSeconds(0.5f);

        if (devilObject != null)
            devilObject.SetActive(true);

        DialogueManager.Instance.playerData.quizCleared = true;

        DialogueManager.Instance.playerData.hasPen = true;
        DialogueManager.Instance.playerData.hasPaper = true;

        DialogueManager.Instance.currentCutscene = CutsceneType.QuizClear;

        DialogueSequence selectedDialogue =
            GameManager_L.Instance != null && GameManager_L.Instance.currentLanguage == Language.EN && quizClearDialogue_EN != null
            ? quizClearDialogue_EN
            : quizClearDialogue;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.FadeOutBGM(1f);
        }
        yield return new WaitForSeconds(1f);

        DialogueManager.Instance.StartDialogue(selectedDialogue);
    }
}