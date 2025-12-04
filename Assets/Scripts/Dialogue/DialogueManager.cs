using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
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

    void Awake()
    {
        dialoguePanel.SetActive(false);
        DontDestroyOnLoad(gameObject); // 전체 씬 공유
    }

    public void StartDialogue(DialogueSequence sequence)
    {
        currentLines = sequence.lines;
        index = 0;

        dialoguePanel.SetActive(true);
        ShowLine();
    }

    void Update()
    {
        if (!dialoguePanel.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            NextLine();
        }
    }

    void ShowLine()
    {
        var line = currentLines[index];

        if (line.requiresName)
        {
            dialoguePanel.SetActive(false);
            nameInputPanel.SetActive(true);
            return;
        }

        string text = line.text
            .Replace("{name}", playerData.playerName)
            .Replace("\\n", "\n");

        // 화자에 따라 색 결정
        string color = "#000000"; // 기본 흰색

        if (line.speaker == "Player")
            color = "#172646";   // 연두색
        else if (line.speaker == "Devil")
            color = "#AB0116";   // 빨간색

        // 말풍선 내용에 색 적용
        dialogueText.text = $"<color={color}>{text}</color>";

        // 화자 이름도 색 줄 수 있음
        //speakerText.text = $"<color={color}>{line.speaker}</color>";
    }

    void NextLine()
    {
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

    void OpenNameInput()
    {
        nameInputPanel.SetActive(true);
        dialoguePanel.SetActive(false);

        nameConfirmButton.onClick.RemoveAllListeners();
        nameConfirmButton.onClick.AddListener(ConfirmName);
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        StartCoroutine(EndSequence());
    }

    [SerializeField] Image fadeImage;
    [SerializeField] TextMeshProUGUI missionText;

    [SerializeField] float fadeDuration = 1.5f;
    IEnumerator EndSequence()
    {
        // 1) 페이드 아웃
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(0f, 1f, t / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, a);
            yield return null;
        }
        yield return new WaitForSeconds(1.5f);

        // 2) 문구 표시
        missionText.gameObject.SetActive(true);

        missionText.text = "악마의 졸업 방해를 이겨내고\n논문을 완성하자!";

        yield return new WaitForSeconds(2.5f);

        trigger.EndCutscene(player);

        // 3) 다음 씬 로드
        UnityEngine.SceneManagement.SceneManager.LoadScene("Room");

        Destroy(gameObject);
    }
}

