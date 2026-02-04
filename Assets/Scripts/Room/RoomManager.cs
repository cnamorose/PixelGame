using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public PlayerData playerdata;

    void Start()
    {
        switch (GameResultHolder.Result)
        {
            case GameResult.Printed:
                OnPaperPrinted();
                break;

            case GameResult.Error:
                OnPaperError();
                break;
        }

        // 결과는 반드시 소모
        GameResultHolder.Result = GameResult.None;
    }

    void OnPaperPrinted()
    {
        // 논문 완료 처리
        playerdata.paperclear = true;

        DialogueManager.Instance.ShowSimpleDialogueAutoClose(
            "논문을 다 작성했다!"
        );
    }

    void OnPaperError()
    {
        DialogueManager.Instance.ShowSimpleDialogueAutoClose(
            "처음부터 다시 작성해야 한다니..."
        );
    }
}
