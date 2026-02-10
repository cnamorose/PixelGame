using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : Interactable
{
    public override void Interact()
    {
        var life = PlayerLifeManager.Instance;

        bool isEN = GameManager_L.Instance.currentLanguage == Language.EN;

        if (life.currentLife >= life.maxLife)
        {

            string text = isEN
                ? "My body is fine, even if my mind feels exhausted..."
                : "정신은 피로해도 몸은 멀쩡하다...";

            DialogueManager.Instance.ShowSimpleDialogueAutoClose(
                text,
                2f
            );
        }
        else
        {

            string question = isEN
                ? "Do you want to sleep and recover?"
                : "자고 일어나시겠습니까?";

            DialogueManager.Instance.ShowChoiceDialogue(
                question,
                onYes: () =>
                {
                    life.FullHeal();
                },
                onNo: () =>
                {
                    // 아무 것도 안 함
                }
            );
        }
    }
}
