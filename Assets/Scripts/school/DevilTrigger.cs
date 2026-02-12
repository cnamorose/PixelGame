using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DialogueManager;

public class DevilTrigger : MonoBehaviour
{
    [Header("Dialogue")]
    public DialogueSequence dialogueKR;
    public DialogueSequence dialogueEN;

    public Transform Demon;
    public Camera mainCamera;
    public GameObject dialogueManagerObj;

    Cameramove cam;
    DialogueManager dlg;
    bool eventStarted = false;

    [Header("BGM")]
    public AudioClip devilBGM;

    PlayerAction player;

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        cam = Camera.main.GetComponent<Cameramove>();
        dlg = dialogueManagerObj.GetComponent<DialogueManager>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (eventStarted) return;
        if (!collision.CompareTag("Player")) return;

        eventStarted = true;

        // PlayerAction 찾기
        player = collision.GetComponentInParent<PlayerAction>();
        if (player == null)
            player = FindObjectOfType<PlayerAction>();

        StartCoroutine(StartDevilEvent());
    }


    IEnumerator StartDevilEvent()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBGM(devilBGM);
        }


        Rigidbody2D rigid = player.GetComponent<Rigidbody2D>();
        rigid.velocity = Vector2.zero;

        player.anim.enabled = false;
        player.forceIdle = true;
        player.idleDir = 1;

        // 카메라 컷씬 이동
        Vector3 mid = (player.transform.position + Demon.position) / 2f;
        mid.z = mainCamera.transform.position.z;
        cam.StartCutscene(mid);

        yield return new WaitForSeconds(0.5f);

        dlg.trigger = this;
        dlg.player = player;

        DialogueManager.Instance.currentCutscene = CutsceneType.SchoolIntro;

        DialogueSequence selectedDialogue =
            GameManager_L.Instance.currentLanguage == Language.KR
            ? dialogueKR
            : dialogueEN;

        dlg.StartDialogue(selectedDialogue);
    }

    public void EndCutscene(PlayerAction player)
    {
        cam.EndCutscene();
        player.forceIdle = false;
        player.anim.enabled = true;
    }
}