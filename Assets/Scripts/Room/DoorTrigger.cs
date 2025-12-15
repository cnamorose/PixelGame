using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorTrigger : MonoBehaviour
{
    public TMP_Text warningText;
    public PlayerData playerData;   // 추가
    public string nextSceneName = "last_school"; // 이동할 씬 이름

    private Coroutine hideRoutine;

    void OnTriggerEnter2D(Collider2D other)
    {

        Debug.Log("문 트리거 진입: " + other.name);

        if (!other.CompareTag("Player"))
            return;

        Debug.Log("플레이어 태그 확인됨");

        if (!other.CompareTag("Player"))
            return;

        // 논문 완료 상태면 씬 이동
        if (playerData.paperclear)
        {
            DialogueManager.Instance.ShowChoiceDialogue(
                "논문을 제출하러 가겠습니까?",
                onYes: () =>
                {
                    SceneManager.LoadScene(nextSceneName);
                },
                onNo: () =>
                {
                    // 아무것도 안 함 (자연스럽게 닫힘)
                }
            );
            return;
        }


        // 아니면 경고 문구
        ShowWarning("논문 작성 전에는 나갈 수 없다...");
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
