using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TypingGameManager : MonoBehaviour
{
    [Header("Warning Blink")]
    public float warningBlinkInterval = 0.7f;

    private Coroutine warningBlinkCoroutine;

    private List<WordMovement> activeWords = new List<WordMovement>();
    private string currentInput = "";

    public TextMeshProUGUI inputUIText;

    [Header("Goal")]
    public int totalTarget = 10;

    [Header("Score")]
    public int successCount = 0;

    [Header("Rules")]
    public int step = 5;
    public int minScore = -10;

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
    public string returnSceneName = "Room";

    [Header("BGM")]
    public AudioClip BGM;

    [Header("SFX")]
    public AudioClip SFX;
    public AudioClip LoopSFX;

    private bool isGameOver = false;


    void Start()
    {
        if (printButton != null)
        {
            PlayerAction.inputLocked = true;
            printButton.gameObject.SetActive(false);
            printButton.onClick.AddListener(OnPrintClicked);
        }

        if (AudioManager.Instance != null && BGM != null)
        {
            AudioManager.Instance.PlayBGM(BGM);
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

        // 혹시 모를 죽은 데이터 미리 청소
        activeWords.RemoveAll(item => item == null);

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
        Debug.Log("Wrong submit");
    }

    // ⭐ 수정: 매개변수로 어떤 단어가 바닥에 닿았는지(this)를 전달받아 지우도록 변경
    public void OnWordMissed(WordMovement missedWord)
    {
        if (isGameOver) return;

        if (missedWord != null && activeWords.Contains(missedWord))
        {
            activeWords.Remove(missedWord);
        }

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
        stage = Mathf.Clamp(stage, 0, progressSprites.Length - 1);
        progressImage.sprite = progressSprites[stage];
    }

    void UpdateGraph()
    {
        if (GraphImage == null || GraphSprites == null || GraphSprites.Length == 0)
            return;

        int graphStage = successCount / 10;
        graphStage = Mathf.Clamp(graphStage, 0, GraphSprites.Length - 1);
        GraphImage.sprite = GraphSprites[graphStage];
    }

    void OnReadyToPrint()
    {
        isGameOver = true;
        PlayerAction.inputLocked = true;
        Time.timeScale = 0f;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopBGM();
        }

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
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopBGM();
        }

        AudioManager.Instance.PlaySFX(SFX);
        yield return new WaitForSecondsRealtime(1.5f);

        Time.timeScale = 1f;
        SceneManager.LoadScene(returnSceneName);
    }

    IEnumerator WarningBlink()
    {
        while (true)
        {
            if (warningImage != null)
            {
                bool nextState = !warningImage.gameObject.activeSelf;
                warningImage.gameObject.SetActive(nextState);

                if (nextState && AudioManager.Instance != null && LoopSFX != null)
                {
                    AudioManager.Instance.PlaySFX(LoopSFX);
                }
            }
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
            AudioManager.Instance.StopLoopingSFX();
            warningBlinkCoroutine = null;
        }

        if (warningImage != null)
            warningImage.gameObject.SetActive(false);
    }
}