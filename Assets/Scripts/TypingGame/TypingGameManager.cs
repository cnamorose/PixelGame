using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TypingGameManager : MonoBehaviour
{
    [Header("Warning Blink")]
    public float warningBlinkInterval = 0.5f;

    private Coroutine warningBlinkCoroutine;

    // 현재 화면에 떨어져 있는 단어들
    private List<WordMovement> activeWords = new List<WordMovement>();

    // 사용자가 입력 중인 문자열
    private string currentInput = "";

    // 입력 UI
    public TextMeshProUGUI inputUIText;

    [Header("Goal")]
    public int totalTarget = 10;

    [Header("Score")]
    public int successCount = 0;   // 맞춘 개수 (음수 가능)

    [Header("Rules")]
    public int step = 5;           // 5단위 스프라이트
    public int minScore = -10;     // -10 되면 게임오버

    [Header("Progress UI")]
    public Image progressImage;
    public Sprite[] progressSprites;

    [Header("Graph UI")]
    public Image GraphImage;
    public Sprite[] GraphSprites;

    [Header("Warning / Error UI")]
    public Image warningImage;
    public Image errorImage;

    [Header("Print")]
    public Button printButton;

    [Header("Scene")]
    public string returnSceneName = "Room"; // 돌아갈 씬 이름

    private bool isGameOver = false;


    void Start()
    {
        if (printButton != null)
        {
            PlayerAction.inputLocked = true;

            printButton.gameObject.SetActive(false);
            printButton.onClick.AddListener(OnPrintClicked);
        }
    }

    void Update()
    {
        if (isGameOver) return;

        foreach (char c in Input.inputString)
        {
            if (c == '\b')
            {
                if (currentInput.Length > 0)
                    currentInput = currentInput[..^1];
            }
            else if (c == '\n' || c == '\r')
            {
                TrySubmitWord();
                currentInput = "";
            }
            else
            {
                if (char.IsLetter(c))
                    currentInput += char.ToLower(c);
            }
        }

        if (inputUIText != null)
            inputUIText.text = currentInput;
    }

    public void RegisterWord(WordMovement word)
    {
        activeWords.Add(word);
    }

    void TrySubmitWord()
    {
        if (currentInput.Length == 0)
            return;

        WordMovement target = null;
        float lowestY = float.MaxValue;

        foreach (WordMovement w in activeWords)
        {
            if (w == null) continue;

            if (w.word == currentInput)
            {
                if (w.GetY() < lowestY)
                {
                    lowestY = w.GetY();
                    target = w;
                }
            }
        }

        if (target != null)
        {
            activeWords.Remove(target);
            Destroy(target.gameObject);
            OnWordSuccess();
        }
        else
        {
            OnWrongSubmit();
        }
    }

    void OnWordSuccess()
    {
        if (isGameOver) return;

        successCount++;

        UpdateProgressSprite();
        UpdateGraph();
        CheckWarningAndError();

        if (successCount >= totalTarget)
        {
            OnReadyToPrint();
        }
    }

    void OnWrongSubmit()
    {
        if (isGameOver) return;

        // 지금은 패널티 없음 (원하면 여기서 successCount-- 해도 됨)
        Debug.Log("Wrong submit");
    }

    // ⭐ 단어를 놓쳤을 때 (WordMovement에서 호출)
    public void OnWordMissed()
    {
        if (isGameOver) return;

        successCount--;

        UpdateProgressSprite();
        UpdateGraph();
        CheckWarningAndError();
    }

    void UpdateProgressSprite()
    {
        if (progressImage == null || progressSprites.Length == 0)
            return;

        int stage = successCount / step;

        // 0 이하는 무조건 0단계 유지
        stage = Mathf.Clamp(stage, 0, progressSprites.Length - 1);

        progressImage.sprite = progressSprites[stage];
    }

    void UpdateGraph()
    {
        if (GraphImage == null || GraphSprites == null || GraphSprites.Length == 0)
            return;

        // 10개 단위로 그래프 단계 계산
        int graphStage = successCount / 10;

        // successCount <= 0 이면 항상 0단계 유지
        graphStage = Mathf.Clamp(graphStage, 0, GraphSprites.Length - 1);

        GraphImage.sprite = GraphSprites[graphStage];
    }

    void OnReadyToPrint()
    {
        isGameOver = true;
        PlayerAction.inputLocked = true;

        // 게임 멈춤
        Time.timeScale = 0f;

        // Print 버튼 표시
        if (printButton != null)
            printButton.gameObject.SetActive(true);
    }

    void OnPrintClicked()
    {
        PlayerAction.inputLocked = false;

        Debug.Log("PRINT COMPLETE");
        GameResultHolder.Result = GameResult.Printed;

        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(returnSceneName);
    }

    void CheckWarningAndError()
    {
        if (successCount <= minScore)
        {
            if (isGameOver) return; 

            isGameOver = true;
            PlayerAction.inputLocked = false;

            Time.timeScale = 0f;

            StopWarningBlink();

            if (warningImage != null)
                warningImage.gameObject.SetActive(false);

            if (errorImage != null)
                errorImage.gameObject.SetActive(true);

            GameResultHolder.Result = GameResult.Error;

            StartCoroutine(ErrorReturnSequence());
            return;
        }

        if (successCount <= -2 && successCount >= -9)
        {
            StartWarningBlink();

            if (errorImage != null)
                errorImage.gameObject.SetActive(false);
        }
        else
        {
            StopWarningBlink();

            if (errorImage != null)
                errorImage.gameObject.SetActive(false);
        }
    }

    IEnumerator ErrorReturnSequence()
    {
        yield return new WaitForSecondsRealtime(1.5f); 

        Time.timeScale = 1f;
        SceneManager.LoadScene(returnSceneName);
    }

    IEnumerator WarningBlink()
    {
        while (true)
        {
            if (warningImage != null)
                warningImage.gameObject.SetActive(!warningImage.gameObject.activeSelf);

            yield return new WaitForSeconds(warningBlinkInterval);
        }
    }

    void StartWarningBlink()
    {
        if (warningBlinkCoroutine != null) return;

        warningBlinkCoroutine = StartCoroutine(WarningBlink());
    }

    void StopWarningBlink()
    {
        if (warningBlinkCoroutine != null)
        {
            StopCoroutine(warningBlinkCoroutine);
            warningBlinkCoroutine = null;
        }

        if (warningImage != null)
            warningImage.gameObject.SetActive(false);
    }
}