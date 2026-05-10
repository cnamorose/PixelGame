using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bookshelf : Interactable
{
    public PlayerData playerdata;

    private int interactCount = 0;
    private bool hasUpgradedLife = false; // 이 책장에서 목숨을 이미 늘렸는지 확인

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
        // ⭐ 중요: maxLife >= 4 체크를 지우고, '이 책장'에서 이미 했는지만 확인합니다.
        if (hasUpgradedLife)
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
            // ⭐ 핵심: 4로 고정하는 대신 현재 최대치에서 1을 더합니다. (3->4 또는 4->5)
            PlayerLifeManager.Instance.maxLife++;
            PlayerLifeManager.Instance.currentLife++;

            // UI 갱신 (알약 칸이 새로 생김)
            PlayerLifeManager.Instance.CallOnLifeChanged();

            hasUpgradedLife = true;

            // 대사가 너무 구체적(4개)이면 어색하므로 범용적으로 수정
            string rewardText = isEN
                ? "Found a hidden vitamin! Maximum Life increased!"
                : "숨겨진 비타민을 발견했다! 최대 목숨이 늘어났다!";

            // 대사 씹힘 방지를 위해 딜레이 호출 (선택 사항)
            StartCoroutine(ShowRewardDelayed(rewardText));
        }
    }

    private IEnumerator ShowRewardDelayed(string text)
    {
        yield return new WaitForSeconds(0.1f);
        DialogueManager.Instance.ShowSimpleDialogueAutoClose(text);
    }
}