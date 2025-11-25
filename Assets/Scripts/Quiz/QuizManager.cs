using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    [Header("Quiz Data")]
    public QuizData[] quizList;       // ScriptableObject 퀴즈들
    private List<QuizData> quizPool;  // 문제 중복 방지용 리스트
    public LifeManager lifeManager;

    private QuizData currentQuiz;

    [Header("UI")]
    public TMP_Text questionText;
    public TMP_Text[] answerTexts;

    [Header("Life System")]
    public Image[] lifePills;         // Pill1, Pill2, Pill3
    public Sprite fullPillSprite;     // 꽉 찬 알약 이미지
    public Sprite emptyPillSprite;    // 빈 알약 이미지

    private int lifeCount = 3;

    [Header("Game Over")]
    public GameObject gameOverPanel;

    private int quizCount = 0;
    private int maxQuizCount = 4;

    void Start()
    {
        quizPool = new List<QuizData>(quizList);
        LoadRandomQuiz();
        UpdateLifeUI();
        gameOverPanel.SetActive(false);
    }

    public void LoadRandomQuiz()
    {
        if (quizCount >= maxQuizCount)
        {
            // ✔ 4문제 다 풀면 게임 종료
            ShowGameOver();
            return;
        }

        if (quizPool.Count == 0)
        {
            Debug.Log("모든 문제를 다 풀었음!");
            ShowGameOver();
            return;
        }

        int rand = Random.Range(0, quizPool.Count);
        currentQuiz = quizPool[rand];

        // UI 적용
        questionText.text = currentQuiz.question;

        for (int i = 0; i < 4; i++)
        {
            answerTexts[i].text = currentQuiz.answers[i];
        }

        // 선택된 문제 제거 → 중복 방지
        //quizPool.RemoveAt(rand);
    }

    public void CheckAnswer(int index)
    {
        if (index == currentQuiz.correctIndex)
        {
            Debug.Log("정답!");

            quizCount++;

            // 다음 문제
            LoadRandomQuiz();
        }
        else
        {
            Debug.Log("오답!");
            lifeManager.LoseLife();
            lifeCount--;
            UpdateLifeUI();

            if (lifeCount <= 0)
            {
                ShowGameOver();
            }
        }
    }

    void UpdateLifeUI()
    {
        for (int i = 0; i < lifePills.Length; i++)
        {
            if (i < lifeCount)
                lifePills[i].sprite = fullPillSprite;
            else
                lifePills[i].sprite = emptyPillSprite;
        }
    }

    void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
    }
}
