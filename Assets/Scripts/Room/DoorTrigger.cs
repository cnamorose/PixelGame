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
        "논문 작성 전에는 나갈 수 없다...",
        "교수님의 환청이 들리는 것 같다'",
        "지금 나가면 Reject의 상처만\n남을 뿐이다.",
        "도망치지 마라.",
        "빈 모니터가 당신을 노려보고 있습니다.",
        "아직 Introduction밖에 안 썼으면서\n어디를 가려고?",
        "등 뒤에서 교수님의\n서늘한 시선이 느껴진다...",
        "디버깅도 안 끝난 코드를 두고 나간다고?",
        "Ctrl + S를 누르지 않은 기억이\n스쳐 지나갔다.",
        "시간은 계속 흘러간다..."
    };

    // 영문 
    private readonly string[] warningsEN = new string[] {
        "You can't leave before finishing the thesis...",
        "I can hear the professor's ghost: 'Is your paper done yet?'",
        "Leaving now will only lead to a brutal Reject.",
        "Don't run away!",
        "The blank screen is staring right into your soul.",
        "You've barely finished the Introduction. Where are you going?",
        "You feel the professor's cold gaze piercing your back...",
        "Leaving before fixing the bugs? Unacceptable.",
        "A sudden horror hits you:\nDid I press Ctrl + S?",
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