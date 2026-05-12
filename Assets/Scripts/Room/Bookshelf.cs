using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bookshelf : Interactable
{
    public PlayerData playerdata;

    private int interactCount = 0;

    public override void Interact()
    {
        bool isEN = GameManager_L.Instance.currentLanguage == Language.EN;

        // 1. 퀴즈 미클리어 시
        if (!playerdata.quizCleared)
        {
            HandleQuizEntrance(isEN);
        }
        // 2. 퀴즈 클리어 후 (이스터 에그)
        else
        {
            HandleEasterEgg(isEN);
        }
    }

    private void HandleQuizEntrance(bool isEN)
    {
        string question = isEN ? "Do you want to enter the quiz stage?" : "퀴즈 스테이지에 입장하시겠습니까?";
        DialogueManager.Instance.ShowChoiceDialogue(
            question,
            onYes: () => { SceneManager.LoadScene("Quiz"); },
            onNo: () => { }
        );
    }

    private void HandleEasterEgg(bool isEN)
    {
        if (playerdata.bookshelfLifeUpgraded)
        {
            string text = isEN ? "It's a bookshelf." : "책장이다.";
            DialogueManager.Instance.ShowSimpleDialogueAutoClose(text);
            return;
        }

        interactCount++;

        if (interactCount <= 3)
        {
            string text = isEN ? "It's a bookshelf." : "책장이다.";
            DialogueManager.Instance.ShowSimpleDialogueAutoClose(text);
        }
        else if (interactCount == 4)
        {
            string text = isEN ? "There is a peculiar book in the bookshelf..." : "책장에 특이한 책이 있다?";
            DialogueManager.Instance.ShowSimpleDialogueAutoClose(text);
        }
        else if (interactCount >= 5)
        {
            string question = isEN ? "Should I open the book?" : "책을 펼쳐볼까?";

            DialogueManager.Instance.ShowChoiceDialogue(
                question,
                onYes: () => {
                    ApplyLifeUpgrade(isEN);
                },
                onNo: () => { }
            );
        }
    }

    private void ApplyLifeUpgrade(bool isEN)
    {
        if (PlayerLifeManager.Instance != null)
        {
            PlayerLifeManager.Instance.maxLife++;
            PlayerLifeManager.Instance.currentLife++;
            PlayerLifeManager.Instance.CallOnLifeChanged();

            // ⭐ 핵심: 영구 데이터인 PlayerData에 저장합니다.
            playerdata.bookshelfLifeUpgraded = true;

            string rewardText = isEN
                ? "Found a hidden vitamin! Maximum Life increased!"
                : "숨겨진 비타민을 발견했다! 최대 목숨이 늘어났다!";

            StartCoroutine(ShowRewardDelayed(rewardText));
        }
    }

    private IEnumerator ShowRewardDelayed(string text)
    {
        yield return new WaitForSeconds(0.1f);
        DialogueManager.Instance.ShowSimpleDialogueAutoClose(text);
    }
}