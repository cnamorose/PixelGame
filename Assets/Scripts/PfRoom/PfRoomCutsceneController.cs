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

    PlayerAction player;

    void Start()
    {
        player = FindObjectOfType<PlayerAction>();

        DialogueManager.Instance.ForceShutdownForSceneChange();
        DialogueManager.Instance.ResetFade();

        StartCoroutine(BeginCutscene());
    }

    IEnumerator BeginCutscene()
    {
        player.LockControl();

        DialogueSequence intro = GetIntroDialogue();

        DialogueManager.Instance.onCutsceneEnd = OnIntroDialogueEnd;
        DialogueManager.Instance.StartDialogue(intro);

        yield break;
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