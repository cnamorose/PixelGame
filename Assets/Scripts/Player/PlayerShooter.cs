using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerShooter : MonoBehaviour
{
    public GameObject penPrefab;
    public Transform firePoint;

    SpriteRenderer sr;
    private int lastDir = 1; // 1 = 오른쪽, -1 = 왼쪽

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // 특정 씬에서만 허용
        //if (SceneManager.GetActiveScene().name != "KeyboardMonster")
        //    return;

        if (SceneManager.GetActiveScene().name != "Keyboard_boss")
            return;

        float h = Input.GetAxisRaw("Horizontal");
        if (h > 0) lastDir = 1;
        else if (h < 0) lastDir = -1;

        if (Input.GetKeyDown(KeyCode.F))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (penPrefab == null || firePoint == null)
        {
            Debug.LogError("penPrefab 또는 firePoint 없음");
            return;
        }

        GameObject pen = Instantiate(
            penPrefab,
            firePoint.position,
            Quaternion.identity
        );

        // 🔹 방향
        Vector2 dir = lastDir == 1 ? Vector2.right : Vector2.left;

        // 🔹 회전 (펜이 수직이라면 90도 돌려야 함)
        float angle = lastDir == 1 ? -90f : 90f;
        pen.transform.rotation = Quaternion.Euler(0, 0, angle);

        PenProjectile proj = pen.GetComponent<PenProjectile>();
        if (proj != null)
        {
            proj.Fire(dir);
        }

        // 🔹 플레이어랑 충돌 무시
        Collider2D penCol = pen.GetComponent<Collider2D>();
        Collider2D playerCol = GetComponent<Collider2D>();
        if (penCol != null && playerCol != null)
        {
            Physics2D.IgnoreCollision(penCol, playerCol);
        }
    }
}
