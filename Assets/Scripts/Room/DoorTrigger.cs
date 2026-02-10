using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorTrigger : MonoBehaviour
{
    public TMP_Text warningText;
    public PlayerData playerData;
    public string nextSceneName = "DevilStart";

    private Coroutine hideRoutine;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        bool isEN = GameManager_L.Instance.currentLanguage == Language.EN;

        if (playerData.paperclear)
        {
            string question = isEN
                ? "Do you want to go submit your thesis?"
                : "논문을 제출하러 가겠습니까?";

            DialogueManager.Instance.ShowChoiceDialogue(
                question,
                onYes: () =>
                {
                    SceneManager.LoadScene(nextSceneName);
                },
                onNo: () =>
                {
                    // 아무것도 안 함
                }
            );
            return;
        }

        string warning = isEN
            ? "You can't leave before finishing the thesis..."
            : "논문 작성 전에는 나갈 수 없다...";

        ShowWarning(warning);
    }

    void ShowWarning(string msg)
    {
        warningText.text = msg;
        warningText.gameObject.SetActive(true);

        if (hideRoutine != null)
            StopCoroutine(hideRoutine);

        hideRoutine = StartCoroutine(Hide());
    }

    IEnumerator Hide()
    {
        yield return new WaitForSeconds(1.5f);
        warningText.gameObject.SetActive(false);
    }
}