using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrisonDoorPerson2 : Interactable
{
    Cameramove cam;

    public GameObject closedPrison;
    public GameObject openedPrison;

    public Person2NPC npc;

    public DialogueSequence rescueDialogue;
    public DialogueManager.CutsceneType cutsceneType
        = DialogueManager.CutsceneType.QuizClear;

    bool isOpened = false;

    public override void Interact()
    {
        if (isOpened) return;

        // 감옥 스프라이트 전환
        if (closedPrison != null) closedPrison.SetActive(false);
        if (openedPrison != null) openedPrison.SetActive(true);

        isOpened = true;

        PlayerAction player =
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAction>();

        cam = Camera.main.GetComponent<Cameramove>();

        // 👉 이번에는 오른쪽으로 이동
        Vector3 backPos = player.transform.position + Vector3.right * 2.5f;

        StartCoroutine(RescueSequence(player, backPos));
    }

    IEnumerator RescueSequence(PlayerAction player, Vector3 backPos)
    {
        // 플레이어 입력 잠금
        player.forceIdle = true;

        // 카메라 컷씬
        if (cam != null)
        {
            Vector3 camTarget =
                (player.transform.position + transform.position) * 0.5f;
            camTarget.z = Camera.main.transform.position.z;

            cam.StartCutscene(camTarget);
        }

        // 플레이어 이동
        yield return player.StartCoroutine(
            player.ForcedMove(backPos, 6f)
        );

        // NPC 이동
        float npcTargetX = backPos.x - 1.5f;
        yield return StartCoroutine(
            npc.FreeAndWait(npcTargetX)
        );

        // 대화 시작
        DialogueManager.Instance.currentCutscene = cutsceneType;
        DialogueManager.Instance.player = player;
        DialogueManager.Instance.StartDialogue(rescueDialogue);
    }
}