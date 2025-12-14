using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCameraTrigger : MonoBehaviour
{
    public KBossController bossController;

    public Transform boss;
    public SpriteRenderer background;   //검은 배경 SpriteRenderer
    public float biasToPlayer = 0.3f;
    public float moveDuration = 0.6f;

    Cameramove cam;
    PlayerAction player;
    bool triggered = false;

    void Start()
    {
        cam = Camera.main.GetComponent<Cameramove>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        // ⭐ 트리거 즉시 비활성화 (다시 안 걸리게)
        GetComponent<Collider2D>().enabled = false;

        // 이하 기존 로직
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

        // ⭐ 컷씬 시작 (여기까지만!)
        cam.StartCutscene(targetPos);

        yield return new WaitForSeconds(moveDuration);

        // ❌ 여기서 절대 cutsceneMode / cutsceneTarget 건들지 마

        if (player != null)
            player.forceIdle = false;

        if (bossController != null)
            bossController.StartBoss();
    }

    Vector3 ClampToBackground(Vector3 pos)
    {
        if (background == null) return pos;

        Camera cam = Camera.main;
        if (cam == null) return pos;

        Bounds b = background.bounds;

        float camHeight = cam.orthographicSize * 2f;
        float camWidth = camHeight * cam.aspect;

        float minX = b.min.x + camWidth / 2f;
        float maxX = b.max.x - camWidth / 2f;
        float minY = b.min.y + camHeight / 2f;
        float maxY = b.max.y - camHeight / 2f;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        return pos;
    }
}