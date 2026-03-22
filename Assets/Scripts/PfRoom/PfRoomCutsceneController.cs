using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PfRoomCutsceneController : MonoBehaviour
{
    [Header("Dialogue")]
    public DialogueSequence introKR;
    public DialogueSequence introEN;
    public DialogueSequence afterKR;
    public DialogueSequence afterEN;

    [Header("Door Sequence")]
    public BackgroundSequencePlayer doorSequence;
    public float doorSequenceLength = 1.1f;

    [Header("SFX")]
    public AudioClip knockSFX;
    public AudioClip doorOpenSFX;

    [Header("Timing")]
    public float sceneStartDelay = 2f;

    PlayerAction player;

    void Start()
    {
        StartCoroutine(BeginSceneLoadedCutscene());
    }

    IEnumerator BeginSceneLoadedCutscene()
    {
        // 씬 완전 전환 안정화
        yield return null;
        yield return new WaitForEndOfFrame();
        yield return null;

        player = FindObjectOfType<PlayerAction>();

        DialogueManager.Instance.ForceShutdownForSceneChange();
        //DialogueManager.Instance.ResetFade();

        // 화면 전환 다 끝난 뒤 2초 대기
        yield return new WaitForSeconds(sceneStartDelay);

        yield return StartCoroutine(BeginCutscene());
    }

    IEnumerator BeginCutscene()
    {
        if (player != null)
            player.LockControl();

        // 노크 소리 먼저 재생
        if (AudioManager.Instance != null && knockSFX != null)
        {
            AudioManager.Instance.PlayOneShotSFX(knockSFX);

            // 소리 끝날 때까지 기다렸다가
            yield return new WaitForSeconds(knockSFX.length);
        }

        // 그 다음 대사 시작
        DialogueSequence intro = GetIntroDialogue();

        DialogueManager.Instance.onCutsceneEnd = OnIntroDialogueEnd;
        DialogueManager.Instance.StartDialogue(intro);
    }

    DialogueSequence GetIntroDialogue()
    {
        bool isEN = GameManager_L.Instance.currentLanguage == Language.EN;
        return isEN ? introEN : introKR;
    }

    DialogueSequence GetAfterDialogue()
    {
        bool isEN = GameManager_L.Instance.currentLanguage == Language.EN;
        return isEN ? afterEN : afterKR;
    }

    void OnIntroDialogueEnd()
    {
        StartCoroutine(OpenDoorSequence());
    }

    IEnumerator OpenDoorSequence()
    {
        if (AudioManager.Instance != null && doorOpenSFX != null)
            AudioManager.Instance.PlayOneShotSFX(doorOpenSFX);

        if (doorSequence != null)
            doorSequence.Play();

        yield return new WaitForSeconds(doorSequenceLength);

        DialogueSequence after = GetAfterDialogue();

        DialogueManager.Instance.onCutsceneEnd = () =>
        {
            SceneManager.LoadScene("Ending");
        };

        DialogueManager.Instance.StartDialogue(after);
    }
}