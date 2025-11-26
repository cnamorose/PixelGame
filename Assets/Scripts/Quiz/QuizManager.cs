using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    public PlayerLifeManager playerLife;

    [Header("Quiz Data")]
    public QuizData[] quizList;       // ScriptableObject 퀴즈들
    private List<QuizData> quizPool;  // 문제 중복 방지용 리스트

    private QuizData currentQuiz;

    [Header("UI")]
    public TMP_Text questionText;
    public TMP_Text[] answerTexts;

    [Header("Life System")]
    public Image[] lifePills;         // Pill1, Pill2, Pill3
    public Sprite fullPillSprite;     // 꽉 찬 알약 이미지
    public Sprite emptyPillSprite;    // 빈 알약 이미지


    [Header("Game Over")]
    public GameObject gameOverPanel;

    private int quizCount = 0;
    private int maxQuizCount = 4;

    void Start()
    {
        quizPool = new List<QuizData>(quizList);
        playerLife = FindObjectOfType<PlayerLifeManager>();
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
            quizCount++;
            LoadRandomQuiz();
        }
        else
        {
            // 플레이어 목숨 감소
            playerLife.LoseLife();

            // UI 업데이트
            UpdateLifeUI();

            // 게임 오버 체크
            if (playerLife.currentLife <= 0)
                ShowGameOver();
        }
    }

    void UpdateLifeUI()
    {
        for (int i = 0; i < lifePills.Length; i++)
        {
            if (i < playerLife.currentLife)
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
