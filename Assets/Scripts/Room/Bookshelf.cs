using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bookshelf : Interactable
{
    public PlayerData playerdata;

    public override void Interact()
    {
        bool isEN = GameManager_L.Instance.currentLanguage == Language.EN;

        if (!playerdata.quizCleared)
        {
            string question = isEN
                ? "Do you want to enter the quiz stage?"
                : "퀴즈 스테이지에 입장하시겠습니까?";

            DialogueManager.Instance.ShowChoiceDialogue(
                question,
                onYes: () =>
                {
                    SceneManager.LoadScene("Quiz");
                },
                onNo: () =>
                {
                    // 아무 것도 안 함
                }
            );
        }
        else
        {
            string text = isEN
                ? "It's a bookshelf."
                : "책장이다.";

            DialogueManager.Instance.ShowSimpleDialogueAutoClose(text);
        }
    }
}
