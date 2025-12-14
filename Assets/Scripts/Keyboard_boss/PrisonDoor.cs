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

    public DialogueSequence rescueDialogue;
    public DialogueManager.CutsceneType cutsceneType
        = DialogueManager.CutsceneType.QuizClear;

    bool isOpened = false;

    public override void Interact()
    {

        if (isOpened) return;

        // PlayerKeyHolder 동적 탐색
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

        // 열쇠 사용
        keyHolder.UseKey();

        // 감옥 스프라이트 전환
        if (closedPrison != null) closedPrison.SetActive(false);
        if (openedPrison != null) openedPrison.SetActive(true);

        isOpened = true;

        PlayerAction player =
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAction>();

        cam = Camera.main.GetComponent<Cameramove>();
        Vector3 camTarget = (player.transform.position + transform.position) * 0.5f;

        camTarget.z = Camera.main.transform.position.z;

        // ⭐ 플레이어를 뒤로 충분히 이동
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

        DialogueManager.Instance.playerData.pcCleared = true;

        DialogueManager.Instance.playerData.hasUsb = true;

        // 대화 시작
        DialogueManager.Instance.currentCutscene = cutsceneType;
        DialogueManager.Instance.player = player;
        DialogueManager.Instance.StartDialogue(rescueDialogue);
    }
}