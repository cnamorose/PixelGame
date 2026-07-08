using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorTrigger : MonoBehaviour
{
    public TMP_Text warningText;
    public PlayerData playerData;

    [Header("처음 보여줄 대화씬")]
    public string firstDialogueSceneName = "DevilStart";

    [Header("대화 본 후 이동할 1번 씬")]
    public string firstStageSceneName = "Stage1";

    private Coroutine hideRoutine;

    // 국문 
    private readonly string[] warningsKR = new string[] {
    "논문 작성 전에는 나갈 수 없다.",
    "졸업 유예 신청하러 가려고?",
    "논문 제목만 들고 졸업할 수 있을까?",
    "도망치지 마라.",
    "빈 모니터가 당신을 노려보고 있습니다.",
    "대학교 5학년 하고 싶어?",
    "멀리서 시선이 느껴진다.",
    "컴퓨터를 켜는 게 더 빠르겠어.",
    "등록금 납부일이 다가오고 있다.",
    "시간은 계속 흘러간다..."
};

    // 영문 
    private readonly string[] warningsEN = new string[] {
    "You can't leave before finishing your thesis...",
    "Going to apply for a delayed graduation?",
    "Can you really graduate with just a thesis title?",
    "Don't run away.",
    "The blank monitor is staring at you.",
    "Want to become a fifth-year college student?",
    "You feel a gaze watching you from afar.",
    "Turning on the computer would be faster.",
    "Want to pay tuition for another semester?",
    "Time keeps ticking away..."
};

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        bool isEN = GameManager_L.Instance != null && GameManager_L.Instance.currentLanguage == Language.EN;

        if (playerData.paperclear)
        {
            string question = isEN
                ? "Do you want to go submit your thesis?"
                : "논문을 제출하러 가겠습니까?";

            DialogueManager.Instance.ShowChoiceDialogue(
                question,
                onYes: () =>
                {
                    if (GameProgressManager.Instance != null &&
                        GameProgressManager.Instance.hasSeenFirstDialogue)
                    {
                        SceneManager.LoadScene(firstStageSceneName);
                    }
                    else
                    {
                        SceneManager.LoadScene(firstDialogueSceneName);
                    }
                },
                onNo: () =>
                {
                    // 아무것도 안 함
                }
            );
            return;
        }

        // 10가지 문장 중 하나를 무작위로 선택 (0 ~ 9)
        int randomIndex = Random.Range(0, 10);
        string warning = isEN ? warningsEN[randomIndex] : warningsKR[randomIndex];

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