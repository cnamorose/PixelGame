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

        // 이름 입력이 필요한 대사라면 → UI 열기
        if (line.requiresName)
        {
            dialoguePanel.SetActive(false);
            nameInputPanel.SetActive(true);
            return;
        }

        // 일반 대사 처리
        string text = line.text.Replace("{name}", playerData.playerName);
        speakerText.text = line.speaker;
        dialogueText.text = text;
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
        trigger.EndCutscene(player);
    }
}

