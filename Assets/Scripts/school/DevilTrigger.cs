using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevilTrigger : MonoBehaviour
{
    public Transform Demon;
    public Camera mainCamera;
    public GameObject dialogueUI;

    bool eventStarted = false;
    Cameramove cam;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        cam = mainCamera.GetComponent<Cameramove>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (eventStarted) return;
        if (!collision.CompareTag("Player")) return;

        eventStarted = true;

        PlayerAction player = collision.GetComponentInParent<PlayerAction>();
        if (player == null)
            return;

        StartCoroutine(StartDevilEvent(player));
    }


    IEnumerator StartDevilEvent(PlayerAction player)
    {
        Rigidbody2D rigid = player.GetComponent<Rigidbody2D>();
        rigid.velocity = Vector2.zero;

        // 🔥 플레이어 움직임 완전정지
        player.anim.enabled = false;

        player.forceIdle = true;
        player.idleDir = 1; // 오른쪽 보기

        // 🔥 카메라 컷씬
        if (cam != null)
        {
            Vector3 mid = (player.transform.position + Demon.position) / 2f;
            mid.z = mainCamera.transform.position.z;

            cam.StartCutscene(mid);
        }

        yield return new WaitForSeconds(0.5f);

        if (dialogueUI)
            dialogueUI.SetActive(true);
    }


    public void EndCutscene(PlayerAction player)
    {
        if (cam != null)
            cam.EndCutscene();

        if (player != null)
        {
            player.forceIdle = false;
            player.anim.enabled = true;
        }    

    }
}