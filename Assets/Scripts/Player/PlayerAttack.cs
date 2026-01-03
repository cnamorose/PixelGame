using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerAttack : MonoBehaviour
{
    public GameObject penHitboxPrefab;
    public Transform attackPoint;
    public float attackDuration = 0.15f;

    PlayerAction action;
    bool isAttacking = false;

    void Awake()
    {
        action = GetComponent<PlayerAction>();
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name != "DevilMonster")
            return;

        if (action.forceIdle || isAttacking)
            return;

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            StartCoroutine(Stab());
        }
    }

    IEnumerator Stab()
    {
        isAttacking = true;
        action.isAttacking = true;

        Vector3 offset = Vector3.right * action.idleDir * 0.5f;

        Quaternion rot =
            action.idleDir == 1
            ? Quaternion.Euler(0, 0, -90f)   // 오른쪽
            : Quaternion.Euler(0, 0, 90f);   // 왼쪽

        Instantiate(
            penHitboxPrefab,
            attackPoint.position + offset,
            rot
        );

        yield return new WaitForSeconds(attackDuration);

        isAttacking = false;
        action.isAttacking = false;
    }
}