using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    public PlayerLifeManager playerLife;

    private float timeLimit = 5f;
    private float currentTime;
    private bool isAnswering = false;
    public TMP_Text timerText;

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

    void Update()
    {
        if (!isAnswering) return;

        currentTime -= Time.deltaTime;

        // 텍스트 UI 갱신
        timerText.text = Mathf.Ceil(currentTime).ToString();

        if (currentTime <= 0)
        {
            TimeOut();
        }
    }

    public void LoadRandomQuiz()
    {
        if (quizCount >= maxQuizCount)
        {
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

        // 문제 중복 방지
        quizPool.RemoveAt(rand);

        questionText.text = currentQuiz.question;

        for (int i = 0; i < 4; i++)
        {
            answerTexts[i].text = currentQuiz.answers[i];
        }

        // 타이머 초기화
        currentTime = timeLimit;
        isAnswering = true;

        timerText.text = currentTime.ToString();
    }

    public void CheckAnswer(int index)
    {
        if (index == currentQuiz.correctIndex)
        {
            isAnswering = false;    
            quizCount++;
            LoadRandomQuiz();
        }
        else
        {  
            playerLife.LoseLife();
            UpdateLifeUI();

            if (playerLife.currentLife <= 0)
            {
                isAnswering = false;   
                ShowGameOver();
            }
        
            else
            {
                LoadRandomQuiz();
            }
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

    void TimeOut()
    {
        isAnswering = false;

        Debug.Log("시간 초과! 목숨 감소");

        playerLife.LoseLife();
        UpdateLifeUI();

        if (playerLife.currentLife <= 0)
        {
            ShowGameOver();
        }
        else
        {
            LoadRandomQuiz();
        }
    }


    void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
    }
}
