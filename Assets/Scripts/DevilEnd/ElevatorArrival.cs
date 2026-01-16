using System.Collections;
using UnityEngine;

enum ElevatorState
{
    Opening,
    Opened,
    Closing,
    Closed
}

public class ElevatorArrival : Interactable
{
    public GameObject open;
    public GameObject close;

    public string openStateName = "Open";
    public string closeStateName = "Close";

    public Transform spawnPoint;

    public float openDelay = 1f; // 씬 시작 후 열리기 전 딜레이

    PlayerAction player;
    ElevatorState state = ElevatorState.Opening;

    void Start()
    {
        player = FindObjectOfType<PlayerAction>();
        StartCoroutine(SceneStartSequence());
    }

    void OnEnable()
    {
        if (open != null) open.SetActive(true);
        if (close != null) close.SetActive(false);

        Animator openAnim = open != null ? open.GetComponent<Animator>() : null;
        if (openAnim != null)
            openAnim.enabled = false;
    }

    IEnumerator SceneStartSequence()
    {
        if (player != null)
        {
            player.transform.position = spawnPoint.position;
            player.LockControl();
        }

        yield return new WaitForSeconds(openDelay);

        Animator openAnim = open != null ? open.GetComponent<Animator>() : null;
        if (openAnim == null)
            yield break;

        openAnim.enabled = true;
        openAnim.speed = 1f;
        openAnim.Play(openStateName, 0, 0f);

        yield return new WaitForSeconds(
            openAnim.GetCurrentAnimatorStateInfo(0).length
        );

        // 마지막 프레임 고정
        openAnim.speed = 0f;

        // 바로 숨김
        if (open != null)
            open.SetActive(false);

        // 플레이어 이동 허용
        if (player != null)
            player.UnlockControl();

        state = ElevatorState.Opened;
    }


    // Exit 트리거에서만 호출
    public void OnPlayerExit()
    {
        if (state != ElevatorState.Opened) return;
        StartCoroutine(CloseSequence());
    }

    IEnumerator CloseSequence()
    {
        state = ElevatorState.Closing;

        // 여기서 open을 완전히 숨김
        if (open != null) open.SetActive(false);
        if (close != null) close.SetActive(true);

        Animator closeAnim = close != null ? close.GetComponent<Animator>() : null;
        if (closeAnim == null)
            yield break;

        closeAnim.enabled = true;
        closeAnim.speed = 1f;
        closeAnim.Play(closeStateName, 0, 0f);

        yield return new WaitForSeconds(
            closeAnim.GetCurrentAnimatorStateInfo(0).length
        );

        // 닫힌 마지막 프레임 고정
        closeAnim.speed = 0f;
        state = ElevatorState.Closed;
    }

    public override void Interact()
    {
        if (state == ElevatorState.Closed)
        {
            DialogueManager.Instance
                .ShowSimpleDialogueAutoClose("수리중입니다.");
        }
    }
}