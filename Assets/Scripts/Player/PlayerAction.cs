using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerAction : MonoBehaviour
{
    [Header("Camera Lock Limit")]
    public bool limitByCamera = false;
    public float camPaddingX = 0.4f; // 좌우 여백
    public float camPaddingY = 0.2f; // 상하 여백

    Vector3 originalScale;

    [Header("Inventory")]
    public GameObject inventoryUI;
    public bool isInventoryOpen = false;

    Interactable currentInteractable;

    public AnimatorOverrideController PlayerM;
    public RuntimeAnimatorController Player;
    public PlayerMoveMode moveMode = PlayerMoveMode.TopDown;

    public float Speed;
    public float jumpForce = 35f;

    public Animator anim;
    Rigidbody2D rigid;

    public float h;
    public float v;
    bool isHorizonMove;
    bool isQuizScene = false;

    public bool forceIdle = false;
    public int idleDir = 1;

    public Transform groundCheck;
    public LayerMask groundLayer;
    bool isGrounded;

    public Transform RespawnPoint_GameOver;
    public Transform PlayerPoint;

    public bool isAttacking = false;
    bool isDevilMonsterScene = false;

    SpriteRenderer sr;

    PlayerPenAttackController penAttack;

    bool isRecoiling = false;

    public static bool inputLocked = false;

    [Header("Footstep")]
    public AudioClip footstepClip;
    public float footstepInterval = 0.4f; // 발소리 간격
    float footstepTimer = 0f;

    [Header("Jump SFX")]
    public AudioClip jumpClip;

    [Header("Monster Hit")]
    public int monsterHitCount = 0;
    public int hitsToLoseLife = 2;
    public float knockbackForceX = 14f;
    public float knockbackForceY = 5f;
    private bool isKnockback = false;

    private bool isInvincible = false; // 직격 데미지용 무적 체크

    public void LockControl()
    {
        Debug.Log("Player Locked");
        forceIdle = true;
        rigid.velocity = Vector2.zero;
        if (anim != null)
            anim.speed = 0f;
    }

    public void UnlockControl()
    {
        Debug.Log("Player Unlocked");
        forceIdle = false;
        if (anim != null)
            anim.speed = 1f;
    }

    public enum PlayerMoveMode
    {
        TopDown,
        Platformer
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("KnockbackMonster"))
        {
            HandleMonsterHit(other.transform);
        }

        Interactable interactable = other.GetComponent<Interactable>();
        if (interactable != null)
        {
            currentInteractable = interactable;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Interactable interactable = other.GetComponent<Interactable>();
        if (interactable != null && currentInteractable == interactable)
        {
            currentInteractable = null;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("KnockbackMonster"))
        {
            HandleMonsterHit(collision.transform);
        }
    }

    public void SetCharacter(string type)
    {
        if (anim == null)
            anim = GetComponent<Animator>();

        GetComponent<SpriteRenderer>().sprite = null;

        if (type == "Boy")
            anim.runtimeAnimatorController = PlayerM;
        else
            anim.runtimeAnimatorController = Player;
    }

    void Awake()
    {
        if (FindObjectsOfType<PlayerAction>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        originalScale = transform.localScale;

        penAttack = GetComponent<PlayerPenAttackController>();
        if (penAttack != null)
            penAttack.enabled = false; // 기본은 꺼둠

        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public IEnumerator ForcedMove(Vector3 targetPos, float speed = 3f)
    {
        forceIdle = true;

        while (Vector3.Distance(transform.position, targetPos) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                speed * Time.deltaTime
            );
            yield return null;
        }

        transform.position = targetPos;
        forceIdle = false;
    }

    void Update()
    {
        HandleFootstepSound();

        if (inputLocked) return;

        if (GameOverManager.Instance != null &&
        GameOverManager.Instance.isGameOverSequenceRunning)
            return;

        // F키 입력 시 현재 씬에 맞는 공격 메서드 실행
        if (Input.GetKeyDown(KeyCode.F))
        {
            HandleUnifiedAttack();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentInteractable != null)
            {
                currentInteractable.Interact();
            }
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            if (inventoryUI == null)
            {
                Debug.LogWarning("InventoryUI 아직 못 찾음");
                return;
            }

            isInventoryOpen = !isInventoryOpen;
            inventoryUI.SetActive(isInventoryOpen);
            forceIdle = isInventoryOpen;
        }


        //if (Input.GetKeyDown(KeyCode.F1))
        //{
        //    SceneManager.LoadScene("ending");
        //    return;
        //}

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Room");
            return;
        }

        // ----------------------------------------------------
        // ⭕ DevilMonster 씬 전용: 방향키 누르면 해당 방향 조준 + 즉시 공격 발동!
        // ----------------------------------------------------
        if (isDevilMonsterScene && SceneManager.GetActiveScene().name == "DevilMonster")
        {
            rigid.velocity = Vector2.zero; // 제자리 고정

            // [왼쪽 화살표] 누르면 왼쪽 보고 즉시 공격
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                idleDir = -1;
                sr.flipX = true;

                // F키를 누른 것과 동일하게 DevilMonster 씬 전용 공격 로직 즉시 실행!
                HandleUnifiedAttack();
            }
            // [오른쪽 화살표] 누르면 오른쪽 보고 즉시 공격
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                idleDir = 1;
                sr.flipX = false;

                // F키를 누른 것과 동일하게 DevilMonster 씬 전용 공격 로직 즉시 실행!
                HandleUnifiedAttack();
            }

            return; // Animator 및 일반 이동 로직 완전 차단
        }

        // 강제 Idle 상태
        if (forceIdle)
        {
            rigid.velocity = Vector2.zero;
            h = 0;
            v = 0;
            anim.SetBool("isChange", false);
            anim.SetInteger("hAxisRaw", idleDir);
            anim.SetInteger("vAxisRaw", 0);
            return;
        }

        if (isQuizScene)
            return;

        // 모드에 따라 입력 분리
        if (moveMode == PlayerMoveMode.TopDown)
            HandleTopDownInput();
        else
            HandlePlatformerInput();

        // 애니메이션 갱신
        if (anim.GetInteger("hAxisRaw") != h)
        {
            anim.SetBool("isChange", true);
            anim.SetInteger("hAxisRaw", (int)h);
        }
        else if (anim.GetInteger("vAxisRaw") != v)
        {
            anim.SetBool("isChange", true);
            anim.SetInteger("vAxisRaw", (int)v);
        }
        else
            anim.SetBool("isChange", false);

        if (SceneManager.GetActiveScene().name == "DevilBoss" && moveMode == PlayerMoveMode.Platformer)
        {
            if (h != 0)
            {
                anim.SetInteger("hAxisRaw", 1);
                anim.SetInteger("vAxisRaw", 0);
                anim.SetBool("isChange", true);
            }
            else
            {
                anim.SetBool("isChange", false);
            }
        }
    }

    void HandleUnifiedAttack()
    {
        if (forceIdle || isInventoryOpen || isQuizScene) return;

        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName == "DevilBoss")
        {
            if (penAttack != null && penAttack.enabled)
            {
                Debug.Log("데빌 보스전: F키로 펜 공격 작동!");
            }
        }
        else if (currentSceneName == "DevilMonster")
        {
            Debug.Log("데빌 몬스터전: F키로 해당 씬 전용 공격 작동!");
        }
        else if (currentSceneName == "KeyboardMonster")
        {
            Debug.Log("키보드 몬스터: F키로 플랫폼 모드 공격 작동!");
        }
        else
        {
            Debug.Log("기본 상태: F키 입력됨");
        }
    }

    void HandleFootstepSound()
    {
        if (moveMode == PlayerMoveMode.Platformer)
        {
            AudioManager.Instance.StopLoopingSFX();
            return;
        }

        bool isMoving = (h != 0 || v != 0);

        if (isMoving)
        {
            AudioManager.Instance.PlayLoopingSFX(footstepClip);
        }
        else
        {
            AudioManager.Instance.StopLoopingSFX();
        }
    }

    private void FixedUpdate()
    {
        if (isRecoiling || isKnockback)
            return;

        if (forceIdle || isQuizScene || isInventoryOpen)
        {
            rigid.velocity = Vector2.zero;
            return;
        }

        // 바닥 체크 (Platformer 모드에서만 사용)
        if (moveMode == PlayerMoveMode.Platformer)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.25f, groundLayer);
            rigid.velocity = new Vector2(h * Speed, rigid.velocity.y);
        }
        else
        {
            Vector2 moveVec = isHorizonMove ? new Vector2(h, 0) : new Vector2(0, v);
            returnVecSpeed(moveVec);
        }
    }

    private void returnVecSpeed(Vector2 moveVec)
    {
        rigid.velocity = moveVec * Speed;
    }

    // ⭐ [수정] WASD를 완전히 차단하고 오직 키보드 방향키만 받도록 변경
    void HandleTopDownInput()
    {
        // 좌우 입력 계산 (오른쪽 화살표 = 1, 왼쪽 화살표 = -1, 둘 다 안 누르면 0)
        float targetH = 0f;
        if (Input.GetKey(KeyCode.RightArrow)) targetH += 1f;
        if (Input.GetKey(KeyCode.LeftArrow)) targetH -= 1f;
        h = targetH;

        // 상하 입력 계산 (위쪽 화살표 = 1, 아래쪽 화살표 = -1, 둘 다 안 누르면 0)
        float targetV = 0f;
        if (Input.GetKey(KeyCode.UpArrow)) targetV += 1f;
        if (Input.GetKey(KeyCode.DownArrow)) targetV -= 1f;
        v = targetV;

        // 누르는 순간(Down)과 떼는 순간(Up)에 방향 우선순위 판단 가공
        bool hDown = Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow);
        bool vDown = Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow);
        bool hUp = Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow);
        bool vUp = Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow);

        if (hDown)
            isHorizonMove = true;
        else if (vDown)
            isHorizonMove = false;
        else if (hUp || vUp)
            isHorizonMove = h != 0;
    }

    // ⭐ [수정] 플랫포머 모드도 WASD를 차단하고 방향키(좌/우)와 Space바 점프로 변경
    void HandlePlatformerInput()
    {
        float targetH = 0f;
        if (Input.GetKey(KeyCode.RightArrow)) targetH += 1f;
        if (Input.GetKey(KeyCode.LeftArrow)) targetH -= 1f;
        h = targetH;
        v = 0;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            Jump();

        if (SceneManager.GetActiveScene().name == "DevilBoss")
        {
            if (h > 0)
            {
                idleDir = 1;
                sr.flipX = false;
            }
            else if (h < 0)
            {
                idleDir = -1;
                sr.flipX = true;
            }
        }
    }

    void Jump()
    {
        rigid.velocity = new Vector2(rigid.velocity.x, jumpForce);

        if (jumpClip != null)
            AudioManager.Instance.PlaySFX(jumpClip);
    }

    void ToggleInventory()
    {
        if (inventoryUI == null)
            return;

        isInventoryOpen = !isInventoryOpen;
        inventoryUI.SetActive(isInventoryOpen);

        if (isInventoryOpen)
        {
            rigid.velocity = Vector2.zero;
            forceIdle = true;
        }
        else
        {
            forceIdle = false;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (penAttack != null)
            penAttack.enabled = false;

        inventoryUI = GameObject.Find("InventoryUI");

        if (inventoryUI != null)
        {
            inventoryUI.SetActive(false);
            isInventoryOpen = false;
        }

        if (scene.name == "Room")
        {
            RespawnPoint_GameOver =
                GameObject.Find("RespawnPoint_GameOver")?.transform;

            PlayerPoint =
                GameObject.Find("PlayerPoint")?.transform;
        }

        if (scene.name == "Room")
        {
            if (GameOverManager.Instance != null &&
                GameOverManager.Instance.fromGameOver)
            {
                if (RespawnPoint_GameOver != null)
                    transform.position = RespawnPoint_GameOver.position;

                if (PlayerLifeManager.Instance != null)
                    PlayerLifeManager.Instance.FullHeal();

                forceIdle = false;
                UnlockControl();

                GameOverManager.Instance.fromGameOver = false;
            }
            else
            {
                if (PlayerPoint != null)
                    transform.position = PlayerPoint.position;
            }
        }
        else if (scene.name == "DevilStart")
        {
            moveMode = PlayerMoveMode.TopDown;
            rigid.gravityScale = 0f;
            rigid.velocity = Vector2.zero;
            rigid.bodyType = RigidbodyType2D.Dynamic;

            transform.localScale = originalScale * 0.5f;
        }

        if (scene.name == "Quiz")
        {
            isQuizScene = true;
            GetComponent<SpriteRenderer>().enabled = false;
            rigid.velocity = Vector2.zero;
        }
        else
        {
            isQuizScene = false;
            GetComponent<SpriteRenderer>().enabled = true;
        }

        if (scene.name == "KeyboardMonster" || scene.name == "Keyboard_boss")
        {
            moveMode = PlayerMoveMode.Platformer;
            rigid.gravityScale = 2.5f;
            rigid.velocity = Vector2.zero;
            transform.localScale = originalScale * 0.5f;
        }
        else if (scene.name == "DevilMonster")
        {
            moveMode = PlayerMoveMode.TopDown;
            rigid.gravityScale = 0f;
            rigid.velocity = Vector2.zero;
            rigid.bodyType = RigidbodyType2D.Kinematic;
            transform.localScale = originalScale * 0.5f;
        }
        else if (scene.name == "DevilBoss")
        {
            if (penAttack != null)
            {
                penAttack.penPivot =
                    GameObject.Find("PenAttackPivot")?.transform;

                penAttack.penHitBox =
                    GameObject.Find("PenHitBox")?.GetComponent<PlayerPenHitBox>();

                if (penAttack.penHitBox != null)
                    penAttack.penHitBox.EnableHitBox(false);
            }

            if (penAttack != null)
                penAttack.enabled = true;

            moveMode = PlayerMoveMode.Platformer;
            rigid.gravityScale = 2.5f;
            rigid.velocity = Vector2.zero;
            rigid.bodyType = RigidbodyType2D.Dynamic;

            forceIdle = false;
            UnlockControl();

            transform.localScale = originalScale * 0.5f;
        }
        else if (scene.name == "devil_end")
        {
            if (penAttack != null)
                penAttack.enabled = false;

            moveMode = PlayerMoveMode.Platformer;
            rigid.gravityScale = 2.5f;
            rigid.velocity = Vector2.zero;
            rigid.bodyType = RigidbodyType2D.Dynamic;

            transform.localScale = originalScale * 0.5f;

            forceIdle = false;
            UnlockControl();

            idleDir = 1;
            sr.flipX = false;

            anim.enabled = true;
            anim.speed = 1f;
            anim.SetInteger("hAxisRaw", 1);
            anim.SetInteger("vAxisRaw", 0);
            anim.SetBool("isChange", false);
        }
        else if (scene.name != "DevilStart")
        {
            moveMode = PlayerMoveMode.TopDown;
            rigid.gravityScale = 0f;
            rigid.velocity = Vector2.zero;
            rigid.bodyType = RigidbodyType2D.Dynamic;

            transform.localScale = originalScale;
        }

        if (scene.name == "DevilMonster")
        {
            isDevilMonsterScene = true;
            h = 0;
            v = 0;
            idleDir = 1;
            sr.flipX = false;

            anim.enabled = true;
            anim.speed = 0f;
            anim.Play("Player_R", 0, 0f);
            anim.Update(0f);

            anim.SetInteger("hAxisRaw", 1);
            anim.SetInteger("vAxisRaw", 0);
            anim.SetBool("isChange", false);

            Transform devilSpawn =
                GameObject.Find("PlayerSpawnPoint")?.transform;

            if (devilSpawn != null)
                transform.position = devilSpawn.position;
        }
        else
        {
            isDevilMonsterScene = false;
            anim.enabled = true;
            anim.speed = 1f;

            if (scene.name != "DevilBoss")
            {
                idleDir = 1;
                sr.flipX = false;
                anim.SetInteger("hAxisRaw", 1);
                anim.SetInteger("vAxisRaw", 0);
                anim.SetBool("isChange", false);
            }
        }
    }

    void LateUpdate()
    {
        if (!limitByCamera) return;

        Camera cam = Camera.main;
        if (cam == null) return;

        Vector3 pos = transform.position;

        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        Vector3 camPos = cam.transform.position;

        float minX = camPos.x - camWidth + camPaddingX;
        float maxX = camPos.x + camWidth - camPaddingX;
        float minY = camPos.y - camHeight + camPaddingY;
        float maxY = camPos.y + camHeight - camPaddingY;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        transform.position = pos;
    }

    IEnumerator ResetChange()
    {
        yield return null;
        anim.SetBool("isChange", false);
    }

    public int GetFacingDir()
    {
        return idleDir;
    }

    void HandleMonsterHit(Transform monster)
    {
        if (isKnockback) return;

        monsterHitCount++;
        Debug.Log("맞은 횟수: " + monsterHitCount);

        Vector2 dir = transform.position.x > monster.position.x
            ? Vector2.right
            : Vector2.left;

        StartCoroutine(Knockback(dir));

        if (monsterHitCount >= hitsToLoseLife)
        {
            monsterHitCount = 0;

            if (PlayerLifeManager.Instance != null)
            {
                PlayerLifeManager.Instance.LoseLife();
            }
        }
    }

    IEnumerator Knockback(Vector2 hitDir)
    {
        isKnockback = true;
        rigid.velocity = Vector2.zero;

        if (anim != null)
            anim.speed = 0f;

        rigid.AddForce(
            new Vector2(hitDir.x * knockbackForceX, knockbackForceY),
            ForceMode2D.Impulse
        );

        float time = 0f;
        float duration = 0.5f;

        while (time < duration)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(0.08f);
            time += 0.08f;
        }

        sr.enabled = true;

        if (anim != null)
            anim.speed = 1f;

        isKnockback = false;
    }

    public void ApplyAttackRecoil()
    {
        if (isRecoiling) return;
        StartCoroutine(AttackRecoilRoutine());
    }

    IEnumerator AttackRecoilRoutine()
    {
        isRecoiling = true;

        rigid.velocity = Vector2.zero;
        rigid.AddForce(
            new Vector2(-idleDir * 2.5f, 0f),
            ForceMode2D.Impulse
        );

        yield return new WaitForSeconds(0.1f);

        isRecoiling = false;
    }

    public void TakeDirectDamage()
    {
        if (isInvincible || isKnockback) return;

        if (PlayerLifeManager.Instance != null)
        {
            PlayerLifeManager.Instance.LoseLife();
        }

        StartCoroutine(DirectDamageInvincibilityRoutine());
    }

    IEnumerator DirectDamageInvincibilityRoutine()
    {
        isInvincible = true;

        float time = 0f;
        float duration = 0.6f;

        while (time < duration)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(0.08f);
            time += 0.08f;
        }

        sr.enabled = true;
        isInvincible = false;
    }
}