using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PC : Interactable
{
    public PlayerData playerdata;

    [Header("PC Screens")]
    public Sprite screen1_Locked;  
    public Sprite screen2_Blue;     
    public Sprite screen3_Clear;

    SpriteRenderer sr;
    [SerializeField] RedFadeController redFade;

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
                "접근 권한이 없습니다"
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

        if (playerdata.pcCleared && !playerdata.paperclear)
        {
            string question = playerdata.paperTried
                ? "논문을 다시 작성하시겠습니까?"
                : "논문을 작성하시겠습니까?";

            DialogueManager.Instance.ShowChoiceDialogue(
                question,
                onYes: () =>
                {
                    playerdata.paperTried = true;
                    SceneManager.LoadScene("TypingGame"); // 타이핑 게임 씬
                },
                onNo: () => { }
            );
            return;
        }

        if (playerdata.paperclear)
        {
            DialogueManager.Instance.ShowSimpleDialogueAutoClose(
            "이미 논문은 완성되어 있다.");
            return;
        }
    }
}
