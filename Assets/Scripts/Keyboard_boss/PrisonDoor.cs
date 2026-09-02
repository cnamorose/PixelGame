using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrisonDoor : Interactable
{
    Cameramove cam;

    public GameObject closedPrison;
    public GameObject openedPrison;

    public Person1NPC senior;
    public PlayerKeyHolder keyHolder;

    [Header("Dialogue")]
    public DialogueSequence bossDeathDialogue;
    public DialogueSequence bossDeathDialogue_EN;

    public DialogueManager.CutsceneType cutsceneType
        = DialogueManager.CutsceneType.QuizClear;

    bool isOpened = false;

    [Header("Objects To Hide")]
    public GameObject[] objectsToHide;

    [Header("SFX")]
    public AudioClip dialogueSFX;

    public override void Interact()
    {
        if (isOpened) return;

        if (keyHolder == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                keyHolder = playerObj.GetComponent<PlayerKeyHolder>();
        }

        if (keyHolder == null || !keyHolder.HasKey())
        {
            DialogueManager.Instance.ShowSimpleDialogue("열쇠가 필요하다...");
            return;
        }

        PlayerAction player =
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAction>();

        StartCoroutine(OpenDoorSequence(player));
    }

    IEnumerator OpenDoorSequence(PlayerAction player)
    {
        isOpened = true;

        // 열쇠 사용 + 효과음
        keyHolder.UseKey();
        AudioManager.Instance.PlayOneShotSFX(dialogueSFX);

    
        yield return new WaitForSeconds(0.4f); 

        // 스프라이트 전환
        if (closedPrison != null) closedPrison.SetActive(false);
        if (openedPrison != null) openedPrison.SetActive(true);

        foreach (GameObject obj in objectsToHide)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        cam = Camera.main.GetComponent<Cameramove>();

        Vector3 backPos = player.transform.position + Vector3.left * 8f;

        StartCoroutine(RescueSequence(player, backPos));
    }

    IEnumerator RescueSequence(PlayerAction player, Vector3 backPos)
    {
        // 플레이어 입력 잠금
        player.forceIdle = true;

        // 카메라 컷씬 시작
        if (cam != null)
        {
            Vector3 camTarget =
                (player.transform.position + transform.position) * 0.5f;
            camTarget.z = Camera.main.transform.position.z;

            cam.StartCutscene(camTarget);
        }

        // 플레이어 뒤로 이동
        yield return player.StartCoroutine(
            player.ForcedMove(backPos, 6f)
        );

        // 선배 이동
        float seniorTargetX = backPos.x + 1.5f;
        yield return StartCoroutine(
            senior.FreeAndWait(seniorTargetX)
        );

        DialogueManager.Instance.playerData.keyboardBossCleared = true;
        DialogueManager.Instance.playerData.hasUsb = true;

        // 대화 시작
        DialogueManager.Instance.currentCutscene = cutsceneType;
        DialogueManager.Instance.player = player;
        DialogueSequence selectedDialogue =
    GameManager_L.Instance.currentLanguage == Language.EN
    && bossDeathDialogue_EN != null
        ? bossDeathDialogue_EN
        : bossDeathDialogue;

        DialogueManager.Instance.StartDialogue(selectedDialogue);
    }
}
