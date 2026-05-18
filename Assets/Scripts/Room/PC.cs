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

        bool isEN = GameManager_L.Instance.currentLanguage == Language.EN;

        // 1️⃣ 퀴즈 미클리어 → 접근 불가
        if (!playerdata.quizCleared)
        {
            string text = isEN
                ? "Access denied."
                : "접근 권한이 없습니다";

            DialogueManager.Instance.ShowSimpleDialogueAutoClose(text);
            return;
        }

        // 2️⃣ PC 미클리어 → 블루스크린 해결?
        if (!playerdata.pcCleared)
        {
            string question = isEN
                ? "It's a blue screen.\nDo you want to fix it?"
                : "블루스크린이다.\n해결하겠습니까?";

            DialogueManager.Instance.ShowChoiceDialogue(
                question,
                onYes: () =>
                {
                    // 🔹 바로 이동하지 않고 코루틴을 호출합니다.
                    StartCoroutine(LoadKeyboardMonsterWithDelay());
                },
                onNo: () =>
                {
                    // 아무 것도 안 함
                }
            );
            return;
        }

        // ⏳ 1초 기다렸다가 씬을 로드하는 함수
        IEnumerator LoadKeyboardMonsterWithDelay()
        {
            yield return new WaitForSeconds(1.0f); // 1초 대기
            SceneManager.LoadScene("KeyboardMonster");
        }

        // 3️⃣ PC 클리어 + 논문 미완성
        if (playerdata.pcCleared && !playerdata.paperclear)
        {
            string question;

            if (playerdata.paperTried)
            {
                question = isEN
                    ? "Do you want to rewrite the thesis?"
                    : "논문을 다시 작성하시겠습니까?";
            }
            else
            {
                question = isEN
                    ? "Do you want to write the thesis?"
                    : "논문을 작성하시겠습니까?";
            }

            DialogueManager.Instance.ShowChoiceDialogue(
                question,
                onYes: () =>
                {
                    playerdata.paperTried = true;
                    StartCoroutine(TypingGameWithDelay());
                },
                onNo: () => { }
            );
            return;
        }

        IEnumerator TypingGameWithDelay()
        {
            yield return new WaitForSeconds(1.0f); // 1초 대기
            SceneManager.LoadScene("TypingGame");
        }

        // 4️⃣ 논문 완료
        if (playerdata.paperclear)
        {
            string text = isEN
                ? "The thesis is already complete."
                : "이미 논문은 완성되어 있다.";

            DialogueManager.Instance.ShowSimpleDialogueAutoClose(text);
            return;
        }
    }
}