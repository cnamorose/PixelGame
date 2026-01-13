using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Elevator : Interactable
{
    public GameObject open;
    public GameObject close;

    public string openStateName = "Open";
    public string closeStateName = "Close";

    public string nextSceneName = "devil_end";

    bool isUsed = false;

    public override void Interact()
    {
        if (isUsed) return;
        isUsed = true;

        PlayerAction player = FindObjectOfType<PlayerAction>();
        StartCoroutine(ElevatorSequence(player));
    }

    void OnEnable()
    {
        if (open != null) open.SetActive(true);
        if (close != null) close.SetActive(false);

        Animator openAnim = open != null ? open.GetComponent<Animator>() : null;
        if (openAnim != null)
            openAnim.enabled = false;
    }

    IEnumerator ElevatorSequence(PlayerAction player)
    {
        if (player != null)
            player.LockControl();

        Animator openAnim = open != null ? open.GetComponent<Animator>() : null;
        if (openAnim == null)
            yield break;

        // 문 열림 애니
        openAnim.enabled = true;
        openAnim.speed = 1f;
        openAnim.Play(openStateName, 0, 0f);

        // normalizedTime 대신 "애니 길이" 기준으로 대기
        yield return new WaitForSeconds(
            openAnim.GetCurrentAnimatorStateInfo(0).length
        );

        // 마지막 프레임 고정
        openAnim.speed = 0f;

        // 닫힘 애니 덮어쓰기
        Animator closeAnim = null;
        if (close != null)
        {
            close.SetActive(true);
            closeAnim = close.GetComponent<Animator>();
            if (closeAnim != null)
            {
                closeAnim.speed = 1f;
                closeAnim.Play(closeStateName, 0, 0f);
            }
        }

        // 닫힘 애니도 끝까지 기다리고 싶으면
        if (closeAnim != null)
        {
            yield return new WaitForSeconds(
                closeAnim.GetCurrentAnimatorStateInfo(0).length
            );
        }

        // 씬 이동
        SceneManager.LoadScene(nextSceneName);
    }
}