using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPenAttackController : MonoBehaviour
{
    [Header("Refs")]
    public Transform penPivot;             // PenAttackPivot
    public PlayerPenHitBox penHitBox;      // PenHitBox

    [Header("Attack")]
    public KeyCode attackKey = KeyCode.J;
    public float swingAngle = 120f;
    public float swingDuration = 0.18f;
    public float cooldown = 0.25f;

    [Header("Visual")]
    public GameObject penVisual;           // PenSprite

    bool isAttacking = false;
    bool attackQueued = false;

    PlayerAction player;                   
    Quaternion defaultRotation;

    void Awake()
    {
        // PlayerAction 참조
        player = GetComponent<PlayerAction>();

        // 기본 회전 저장 (씬에서 세팅한 값)
        if (penPivot != null)
            defaultRotation = penPivot.localRotation;

        // 기본 상태: 펜 숨김
        if (penVisual != null)
            penVisual.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(attackKey))
        {
            if (!isAttacking)
                StartCoroutine(Swing());
            else
                attackQueued = true;
        }
    }

    IEnumerator Swing()
    {
        Debug.Log("ATTACK DIR = " + player.GetFacingDir());

        isAttacking = true;

        // 펜 보이기
        if (penVisual != null)
            penVisual.SetActive(true);

        // 히트박스 ON
        penHitBox.EnableHitBox(true);

        float half = swingAngle * 0.5f;
        float t = 0f;

        // PlayerAction에서 관리하는 방향
        int dir = player.GetFacingDir();
        // 1 = 오른쪽, -1 = 왼쪽

        float baseAngle = defaultRotation.eulerAngles.z;
        float from, to;

        if (dir == 1)
        {
            // 오른쪽 공격
            from = baseAngle + half;
            to = baseAngle - half;
        }
        else
        {
            // 왼쪽 공격
            from = baseAngle - half;
            to = baseAngle + half;
        }

        while (t < swingDuration)
        {
            t += Time.deltaTime;
            float angle = Mathf.Lerp(from, to, t / swingDuration);
            penPivot.localRotation = Quaternion.Euler(0, 0, angle);
            yield return null;
        }

        // 히트박스 OFF
        penHitBox.EnableHitBox(false);

        // 펜 숨기기
        if (penVisual != null)
            penVisual.SetActive(false);

        // 기본 회전 복구
        penPivot.localRotation = defaultRotation;

        yield return new WaitForSeconds(cooldown);

        isAttacking = false;

        // 입력 버퍼 처리
        if (attackQueued)
        {
            attackQueued = false;
            StartCoroutine(Swing());
        }
    }
}