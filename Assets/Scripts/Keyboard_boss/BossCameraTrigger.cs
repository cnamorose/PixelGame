using System.Collections;
using UnityEngine;

public class BossCameraTrigger : MonoBehaviour
{
    public KBossController bossController;

    [Header("BGM")]
    public AudioClip bossBGM;

    public Transform boss;
    public SpriteRenderer background;
    public float moveDuration = 0.6f;

    Cameramove cam;
    PlayerAction player;
    bool triggered = false;

    [Header("Dialogue")]
    public DialogueSequence bossIntroDialogue;
    public DialogueSequence bossIntroDialogue_EN;

    void Start()
    {
        cam = Camera.main.GetComponent<Cameramove>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;
        GetComponent<Collider2D>().enabled = false;

        player = other.GetComponent<PlayerAction>();
        if (player != null)
        {
            player.forceIdle = true;
            player.limitByCamera = true;
        }

        StartCoroutine(MoveCameraAndLock());
    }

    IEnumerator MoveCameraAndLock()
    {
        if (cam == null || boss == null) yield break;

        Camera unityCam = Camera.main;
        float halfWidth = unityCam.orthographicSize * unityCam.aspect;
        float playerScreenRatio = 0.25f;

        Vector3 targetPos = boss.position;
        targetPos.x -= halfWidth * (1f - playerScreenRatio);
        targetPos.x += 2.0f;
        targetPos.z = unityCam.transform.position.z;

        cam.StartCutscene(targetPos);
        yield return new WaitForSeconds(moveDuration);

        // ⭐ 이미 대화를 봤는지 체크
        if (GameProgressManager.Instance != null && GameProgressManager.Instance.hasSeenBossIntro)
        {
            OnBossDialogueEnd();
        }
        else
        {
            DialogueManager.Instance.onCutsceneEnd = OnBossDialogueEnd;
            DialogueSequence selected = GameManager_L.Instance.currentLanguage == Language.EN
                                        ? bossIntroDialogue_EN : bossIntroDialogue;
            DialogueManager.Instance.StartDialogue(selected);
        }
    }

    void OnBossDialogueEnd()
    {
        if (GameProgressManager.Instance != null)
            GameProgressManager.Instance.hasSeenBossIntro = true;

        if (AudioManager.Instance != null && bossBGM != null)
            AudioManager.Instance.PlayBGM(bossBGM);

        if (player != null) player.forceIdle = false;

        if (bossController != null) bossController.StartBoss();
    }
}