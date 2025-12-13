using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : Interactable
{
    public override void Interact()
    {
        // 안전 체크 (혹시 모를 상황 대비)
        if (PlayerLifeManager.Instance == null)
            return;

        // 체력이 가득 찼을 때
        if (PlayerLifeManager.Instance.currentLife >=
            PlayerLifeManager.Instance.maxLife)
        {
            DialogueManager.Instance.ShowSimpleDialogue(
                "정신은 피로해도 몸은 멀쩡하다..."
            );
        }
        else
        {
            DialogueManager.Instance.ShowSimpleDialogue(
                "10분만 자고 일어나자..."
            );

            PlayerLifeManager.Instance.FullHeal();
        }
    }
}
