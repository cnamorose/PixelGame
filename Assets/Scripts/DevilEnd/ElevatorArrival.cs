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
    public Animator closeAnim;
    public string closeStateName = "Close";

    public Transform spawnPoint;
    PlayerAction player;

    public float openAnimTime = 0.4f;   // 닫힘 애니 길이와 동일하게

    ElevatorState state = ElevatorState.Opening;

    void Start()
    {
        player = FindObjectOfType<PlayerAction>();
        StartCoroutine(SceneStartSequence());
    }

    IEnumerator SceneStartSequence()
    {
        if (player != null)
        {
            player.transform.position = spawnPoint.position;
            player.LockControl();
        }

        // ✅ Animator 준비 한 프레임 대기 (중요)
        yield return null;

        // 닫힌 상태에서 시작 → 역재생으로 열기
        closeAnim.enabled = true;
        closeAnim.speed = -1f;
        closeAnim.Play(closeStateName, 0, 1f);

        // ❌ normalizedTime 쓰지 말고 시간으로 기다림
        yield return new WaitForSeconds(openAnimTime);

        // 마지막 프레임 고정
        closeAnim.speed = 0f;
        state = ElevatorState.Opened;

        if (player != null)
            player.UnlockControl();
    }

    // 트리거에서 호출
    public void OnPlayerExit()
    {
        if (state != ElevatorState.Opened) return;
        StartCoroutine(CloseSequence());
    }

    IEnumerator CloseSequence()
    {
        state = ElevatorState.Closing;

        closeAnim.speed = 1f;
        closeAnim.Play(closeStateName, 0, 0f);

        yield return new WaitForSeconds(openAnimTime);

        closeAnim.speed = 0f;
        state = ElevatorState.Closed;
    }

    public override void Interact()
    {
        if (state == ElevatorState.Closed)
        {
            DialogueManager.Instance.ShowSimpleDialogueAutoClose("수리중입니다.");
        }
    }
}