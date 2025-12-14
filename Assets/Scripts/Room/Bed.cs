using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : Interactable
{
    public override void Interact()
    {
        var life = PlayerLifeManager.Instance;

        Debug.Log("침대 상호작용");

        if (life.currentLife >= life.maxLife)
        {
            DialogueManager.Instance.ShowSimpleDialogueAutoClose(
                "정신은 피로해도 몸은 멀쩡하다...",
                2f
            );
        }
        else
        {
            DialogueManager.Instance.ShowChoiceDialogue(
                "자고 일어나시겠습니까?",
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
