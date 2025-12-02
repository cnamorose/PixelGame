using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
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
        DontDestroyOnLoad(gameObject); // ÀüÃ¼ ¾À °øÀ¯
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
        speakerText.text = currentLines[index].speaker;
        dialogueText.text = currentLines[index].text;
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

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        trigger.EndCutscene(player);
    }
}

