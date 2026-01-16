using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    // ⭐ 대화 상태 하나로 통합
    public enum DialogueMode
    {
        None,
        Simple,     // 단발 대사
        Choice,     // 선택지
        Cutscene    // 컷신 (끝나면 EndSequence)
    }

    DialogueMode mode = DialogueMode.None;

    [Header("Choice UI")]
    public GameObject choicePanel;
    public Button yesButton;
    public Button noButton;

    public PlayerData playerData;
    public GameObject nameInputPanel;
    public TMP_InputField nameInputField;
    public Button nameConfirmButton;

    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI dialogueText;
    public GameObject dialoguePanel;

    List<DialogueLine> currentLines;
    int index = 0;

    public DevilTrigger trigger;
    public PlayerAction player;

    [Header("Cutscene UI")]
    [SerializeField] Image fadeImage;
    [SerializeField] TextMeshProUGUI missionText;
    [SerializeField] float fadeDuration = 1.5f;

    public Action onCutsceneEnd;

    void Awake()
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

        dialoguePanel.SetActive(false);
        choicePanel.SetActive(false);
        nameInputPanel.SetActive(false);

        if (missionText != null)
            missionText.gameObject.SetActive(false);

        if (fadeImage != null)
            fadeImage.color = new Color(0, 0, 0, 0);
    }

    void EnsurePlayer()
    {
        if (player == null)
        {
            player = FindObjectOfType<PlayerAction>();

            if (player == null)
                Debug.LogWarning("DialogueManager: PlayerAction not found");
        }
    }

    /* =========================
       대화 시작 함수들
       ========================= */

    // 컷신 대화 시작
    public void StartDialogue(DialogueSequence sequence)
    {
        EnsurePlayer();

        StopAllCoroutines();

        mode = DialogueMode.Cutscene;
        currentLines = sequence.lines;
        index = 0;

        dialoguePanel.SetActive(true);
        choicePanel.SetActive(false);

        if (player != null)
            player.LockControl();

        ShowLine();
    }

    // 단발 대사
    public void ShowSimpleDialogue(string text, string color = "#000000")
    {
        EnsurePlayer();

        StopAllCoroutines();

        mode = DialogueMode.Simple;
        currentLines = null;

        dialoguePanel.SetActive(true);
        choicePanel.SetActive(false);

        if (player != null)
            player.LockControl();

        dialogueText.text = $"<color={color}>{text}</color>";
    }

    // 자동 닫힘 단발 대사
    public void ShowSimpleDialogueAutoClose(string text, float closeTime = 2f, string color = "#000000")
    {
        EnsurePlayer();

        StopAllCoroutines();

        mode = DialogueMode.Simple;
        currentLines = null;

        dialoguePanel.SetActive(true);
        choicePanel.SetActive(false);

        if (player != null)
            player.LockControl();

        dialogueText.text = $"<color={color}>{text}</color>";
        StartCoroutine(AutoCloseDialogue(closeTime));
    }

    // 선택지 대사
    public void ShowChoiceDialogue(string text, Action onYes, Action onNo, string color = "#000000")
    {
        EnsurePlayer();

        StopAllCoroutines();

        mode = DialogueMode.Choice;
        currentLines = null;

        dialoguePanel.SetActive(true);
        choicePanel.SetActive(true);

        if (player != null)
            player.LockControl();

        dialogueText.text = $"<color={color}>{text}</color>";

        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();

        yesButton.onClick.AddListener(() =>
        {
            onYes?.Invoke();
            CloseDialogue();
        });

        noButton.onClick.AddListener(() =>
        {
            onNo?.Invoke();
            CloseDialogue();
        });
    }

    /* =========================
       Update
       ========================= */

    void Update()
    {
        if (!dialoguePanel.activeSelf) return;

        if (mode == DialogueMode.Choice)
            return;

        if (mode == DialogueMode.Simple)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                CloseDialogue();
            return;
        }

        if (mode == DialogueMode.Cutscene)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                NextLine();
        }
    }

    /* =========================
       컷신 흐름
       ========================= */

    void ShowLine()
    {
        var line = currentLines[index];

        if (line.requiresName)
        {
            dialoguePanel.SetActive(false);
            nameInputPanel.SetActive(true);
            return;
        }

        if (line.shakeCamera)
        {
            Cameramove cam = Camera.main?.GetComponent<Cameramove>();
            if (cam != null)
            {
                float magnitude = line.weakShake ? 0.08f : 0.2f;
                StartCoroutine(cam.ShakeCamera(1.5f, magnitude));
            }
        }

        string text = line.text
            .Replace("{name}", playerData.playerName)
            .Replace("\\n", "\n");

        string color = "#000000";

        if (line.speaker == "Player")
            color = "#172646";
        else if (line.speaker == "Devil")
            color = "#AB0116";
        else if (line.speaker == "person")
            color = "#3a6b4f";


        dialogueText.text = $"<color={color}>{text}</color>";
    }

    void NextLine()
    {
        // ⭐ 지금 줄이 끝나는 순간
        var prevLine = currentLines[index];

        if (prevLine.changeDevilSpriteOnEnd && prevLine.devilSpriteOnEnd != null)
        {
            DevilVisual devil = FindObjectOfType<DevilVisual>();
            if (devil != null)
                devil.SetSprite(prevLine.devilSpriteOnEnd);
        }

        index++;

        if (index < currentLines.Count)
        {
            ShowLine();
        }
        else
        {
            EndDialogue();
        }
    }

    public void ConfirmName()
    {
        string name = nameInputField.text;
        if (string.IsNullOrWhiteSpace(name))
            name = "플레이어";

        playerData.playerName = name;

        nameInputPanel.SetActive(false);
        dialoguePanel.SetActive(true);

        NextLine();
    }

    /* =========================
       종료 처리
       ========================= */

    public void CloseDialogue()
    {
        StopAllCoroutines();

        dialoguePanel.SetActive(false);
        choicePanel.SetActive(false);

        if (mode == DialogueMode.Cutscene && onCutsceneEnd != null)
        {
            onCutsceneEnd.Invoke();
            onCutsceneEnd = null;
        }

        mode = DialogueMode.None;
        currentLines = null;

        if (player != null)
            player.UnlockControl();
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);

        if (mode == DialogueMode.Cutscene)
        {
            if(onCutsceneEnd != null)
            {
                onCutsceneEnd.Invoke();
                onCutsceneEnd = null;
            }
            else
            {
                StartCoroutine(EndSequence());
            }
        }

        mode = DialogueMode.None;

        if (player != null)
            player.UnlockControl();
    }


    IEnumerator AutoCloseDialogue(float time)
    {
        yield return new WaitForSeconds(time);
        CloseDialogue();
    }
    IEnumerator FadeInAfterSceneLoad()
    {
        // 씬 로드 기다림
        yield return null;
        yield return new WaitForSeconds(0.1f);

        // 미션 문구 숨김
        if (missionText != null)
            missionText.gameObject.SetActive(false);

        // 페이드 인
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(1f, 0f, t / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, a);
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, 0);
    }

    public enum CutsceneType
    {
        None,
        SchoolIntro,
        QuizClear,
        KeyMonster,
        DevilMonster,
        DevilEnd,
        PfRoom
    }

    public CutsceneType currentCutscene = CutsceneType.None;
    public void ForceShutdownForSceneChange()
    {

        dialoguePanel.SetActive(false);
        choicePanel.SetActive(false);
        nameInputPanel.SetActive(false);

        mode = DialogueMode.None;
        currentLines = null;
        index = 0;
    }
    IEnumerator EndSequence()
    {
        // ⭐ KeyMonster 컷신 전용: 먼저 흔들림 시작
        if (currentCutscene == CutsceneType.KeyMonster)
        {
            Cameramove cam = Camera.main ? Camera.main.GetComponent<Cameramove>() : null;
            if (cam != null)
            {
                // 페이드 시간 + 약간 더
                StartCoroutine(cam.ShakeCamera(fadeDuration + 0.3f, 0.2f));
            }
        }

        // =====================
        // 공통: 검정 페이드 아웃
        // =====================
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(0f, 1f, t / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, a);
            yield return null;
        }

        // =====================
        // School 컷신 전용
        // =====================
        if (currentCutscene == CutsceneType.SchoolIntro)
        {
            yield return new WaitForSeconds(1.5f);

            missionText.gameObject.SetActive(true);
            missionText.text = "악마의 졸업 방해를 이겨내고\n논문을 완성하자!";

            yield return new WaitForSeconds(2.5f);

            trigger.EndCutscene(player);
        }

        // =====================
        // Quiz 컷신 전용
        // =====================
        if (currentCutscene == CutsceneType.QuizClear)
        {
            yield return new WaitForSeconds(0.5f);
        }

        // =====================
        // DevilMonster 컷신 전용
        // =====================
        if (currentCutscene == CutsceneType.DevilMonster)
        {
            ForceShutdownForSceneChange();
            yield return new WaitForSeconds(0.5f);
            SceneManager.LoadScene("DevilBoss");
            yield break;
        }

        if (currentCutscene == CutsceneType.DevilEnd)
        {
            yield return new WaitForSeconds(0.5f);
            SceneManager.LoadScene("school_run");
            StartCoroutine(FadeInAfterSceneLoad());
            yield break;
        }

        // =====================
        // 공통: 씬 이동 + 페이드 인
        // =====================
        currentCutscene = CutsceneType.None;

        Cameramove cam2 = Camera.main ? Camera.main.GetComponent<Cameramove>() : null;
        if (cam2 != null)
            cam2.EndCutscene();

        if (player != null)
            player.forceIdle = false;

        UnityEngine.SceneManagement.SceneManager.LoadScene("Room");
        StartCoroutine(FadeInAfterSceneLoad());
    }

    public void PlayCutsceneFadeOnly(CutsceneType type)
    {
        StopAllCoroutines();

        currentCutscene = type;
        mode = DialogueMode.Cutscene;

        StartCoroutine(EndSequence());
    }


}
