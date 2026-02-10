using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSceneTrigger : MonoBehaviour
{
    Cameramove cam;

    public Transform cameraFocus;

    public DialogueManager.CutsceneType cutsceneType; 

    public GameObject[] objectsToHide;

    [Header("Dialogue")]
    public DialogueSequence bossDeathDialogue;
    public DialogueSequence bossDeathDialogue_EN;

    bool triggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        PlayerAction player = other.GetComponent<PlayerAction>();
        if (player == null) return;

        cam = Camera.main.GetComponent<Cameramove>();

        Vector3 camTarget = cameraFocus.position;
        camTarget.z = Camera.main.transform.position.z;

        player.forceIdle = true;

        foreach (GameObject obj in objectsToHide)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        cam.StartCutscene(camTarget);

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
