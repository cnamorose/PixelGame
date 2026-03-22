using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public PlayerData playerdata;

    void Start()
    {
        PlayerLifeManager.Instance.ShowPlayerAgain();

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

        bool isEN = GameManager_L.Instance.currentLanguage == Language.EN;

        string text = isEN
            ? "The thesis is finally complete!"
            : "논문을 다 작성했다!";

        DialogueManager.Instance.ShowSimpleDialogueAutoClose(text);
    }

    void OnPaperError()
    {
        bool isEN = GameManager_L.Instance.currentLanguage == Language.EN;

        string text = isEN
            ? "I have to start all over again..."
            : "처음부터 다시 작성해야 한다니...";

        DialogueManager.Instance.ShowSimpleDialogueAutoClose(text);
    }
}
