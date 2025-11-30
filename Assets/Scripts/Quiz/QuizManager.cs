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

    [Header("Fade UI")]
    public Image fadePanel;

    [Header("Game Over")]
    public GameObject gameOverPanel;

    [Header("UI Group")]
    public GameObject lifeUI;     // 알약 UI 묶은 부모 오브젝트
    public GameObject timerUI;
    public TMP_Text extraGameOverText;

    private int quizCount = 0;
    private int maxQuizCount = 4;

    void Start()
    {
        quizPool = new List<QuizData>(quizList);
        playerLife = FindObjectOfType<PlayerLifeManager>();
        LoadRandomQuiz();
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

    void TimeOut()
    {
        isAnswering = false;

        Debug.Log("시간 초과! 목숨 감소");

        playerLife.LoseLife();

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
        StartCoroutine(FadeAndShowGameOver());
    }

    IEnumerator FadeAndShowGameOver()
    {
        lifeUI.SetActive(false);
        timerUI.SetActive(false);

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
