using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bookshelf : Interactable
{
    public PlayerData playerdata;

    public override void Interact()
    {
        if (!playerdata.quizCleared)
        {
            DialogueManager.Instance.ShowChoiceDialogue(
                "퀴즈 스테이지에 입장하시겠습니까?",
                onYes: () =>
                {
                    SceneManager.LoadScene("Quiz");
                },
                onNo: () =>
                {
                }
            );
            return;
        }
        else
        {
            DialogueManager.Instance.ShowSimpleDialogueAutoClose(
                "책장이다."
            );
            return;
        }
    }
}
