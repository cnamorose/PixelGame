using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : Interactable
{
    private int interactCount = 0;
    private bool hasUpgradedMaxLife = false; // 이 창문에서 보상을 이미 받았는지 확인

    public override void Interact()
    {
        var life = PlayerLifeManager.Instance;
        bool isEN = GameManager_L.Instance.currentLanguage == Language.EN;

        // 이미 이 창문에서 목숨을 늘렸다면 평범한 대사만 출력
        if (hasUpgradedMaxLife)
        {
            string text = isEN ? "The sunlight is warm." : "햇살이 따스하다.";
            DialogueManager.Instance.ShowSimpleDialogueAutoClose(text);
            return;
        }

        interactCount++;

        if (interactCount <= 5)
        {
            // 1~5번째 클릭: 창문이다.
            string text = isEN ? "It's a window." : "창문이다.";
            DialogueManager.Instance.ShowSimpleDialogueAutoClose(text);
        }
        else if (interactCount == 6)
        {
            // 6번째 클릭: 몸이 가벼워지는데?
            string text = isEN ? "My body feels lighter?" : "몸이 가벼워지는데?";
            DialogueManager.Instance.ShowSimpleDialogueAutoClose(text);
        }
        else if (interactCount >= 7)
        {
            // 7번째 클릭 이상: 햇빛으로 비타민 d를 충족했다!
            ApplyWindowUpgrade(life, isEN);
        }
    }

    private void ApplyWindowUpgrade(PlayerLifeManager life, bool isEN)
    {
        // ⭐ 핵심: 현재 maxLife가 3이든 4든 상관없이 +1을 해줍니다.
        life.maxLife++;
        life.currentLife++; // 현재 체력도 보너스로 1칸 회복

        // UI 방송 (UI 매니저에게 알약 칸을 새로 그리라고 명령)
        life.CallOnLifeChanged();

        hasUpgradedMaxLife = true;

        string rewardText = isEN
            ? "Replenished Vitamin D with sunlight! Max Life increased!"
            : "햇빛으로 비타민 D를 충족했다! 최대 목숨이 늘어났다!";

        // 대사 씹힘 방지 딜레이
        StartCoroutine(ShowRewardDelayed(rewardText));
    }

    private IEnumerator ShowRewardDelayed(string text)
    {
        yield return new WaitForSeconds(0.1f);
        DialogueManager.Instance.ShowSimpleDialogueAutoClose(text);
    }
}