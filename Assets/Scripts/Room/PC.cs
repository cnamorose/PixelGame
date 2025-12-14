using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PC : Interactable
{
    public PlayerData playerdata;

    [Header("PC Screens")]
    public Sprite screen1_Locked;  
    public Sprite screen2_Blue;     
    public Sprite screen3_Clear;

    SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        UpdateScreen();
    }
    void UpdateScreen()
    {
        if (playerdata.pcCleared)
        {
            sr.sprite = screen3_Clear;
        }
        else if (playerdata.quizCleared)
        {
            sr.sprite = screen2_Blue;
        }
        else
        {
            sr.sprite = screen1_Locked;
        }
    }
    public override void Interact()
    {
        Debug.Log("PC Interact 호출됨");

        if (!playerdata.quizCleared)
        {
            DialogueManager.Instance.ShowSimpleDialogueAutoClose(
                "다른 스테이지를 클리어해야 한다..."
            );
            return;
        }

        if (!playerdata.pcCleared)
        {
            DialogueManager.Instance.ShowChoiceDialogue(
                "블루스크린이다.\n해결하겠습니까?",
                onYes: () =>
                {
                    SceneManager.LoadScene("KeyboardMonster");
                },
                onNo: () =>
                {
                    // 아무 것도 안 함
                }
            );
            return;
        }

        DialogueManager.Instance.ShowSimpleDialogueAutoClose(
            "이미 해결된 문제다."
        );
    }
}
