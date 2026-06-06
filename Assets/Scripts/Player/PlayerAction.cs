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

        // ⭐ [핵심 추가] F키 입력 시 현재 씬에 맞는 공격 메서드 실행
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

        // 씬 이동 단축키
        if (Input.GetKeyDown(KeyCode.K))
        {
            SceneManager.LoadScene("KeyboardMonster");
            return;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            SceneManager.LoadScene("devil_end");
            return;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Room");
            return;
        }

        if (isDevilMonsterScene && SceneManager.GetActiveScene().name == "DevilMonster")
        {
            rigid.velocity = Vector2.zero;

            if (Input.GetKeyDown(KeyCode.A))
            {
                idleDir = -1;
                sr.flipX = true;
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                idleDir = 1;
                sr.flipX = false;
            }

            return; // Animator 로직 완전 차단
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

    // ⭐ [추가] F키 하나로 모든 씬의 공격을 제어하는 분기 메서드
    void HandleUnifiedAttack()
    {
        // 인벤토리가 열려있거나 강제 Idle 상태면 공격 불가
        if (forceIdle || isInventoryOpen || isQuizScene) return;

        string currentSceneName = SceneManager.GetActiveScene().name;

        // 1. 데빌 보스 씬일 때 (펜 공격 스크립트 연동)
        if (currentSceneName == "DevilBoss")
        {
            if (penAttack != null && penAttack.enabled)
            {
                // 만약 PlayerPenAttackController 내부에 실행 함수(예: Attack())가 따로 있다면 
                // 여기서 penAttack.Attack(); 형태로 직접 호출해 줘도 됩니다.
                Debug.Log("데빌 보스전: F키로 펜 공격 작동!");
            }
        }
        // 2. 데빌 몬스터 씬일 때 (필요시 전용 로직 작성)
        else if (currentSceneName == "DevilMonster")
        {
            Debug.Log("데빌 몬스터전: F키로 해당 씬 전용 공격 작동!");
            // TODO: DevilMonster 씬 전용 공격 함수가 있다면 여기에 매핑
        }
        // 3. 키보드 몬스터 맵일 때
        else if (currentSceneName == "KeyboardMonster")
        {
            Debug.Log("키보드 몬스터: F키로 플랫폼 모드 공격 작동!");
        }
        // 4. 그 외 기본 상태일 때
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
            rigid.velocity = moveVec * Speed;
        }
    }

    void HandleTopDownInput()
    {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        bool hDown = Input.GetButtonDown("Horizontal");
        bool vDown = Input.GetButtonDown("Vertical");
        bool hUp = Input.GetButtonUp("Horizontal");
        bool vUp = Input.GetButtonUp("Vertical");

        if (hDown)
            isHorizonMove = true;
        else if (vDown)
            isHorizonMove = false;
        else if (hUp || vUp)
            isHorizonMove = h != 0;
    }

    void HandlePlatformerInput()
    {
        h = Input.GetAxisRaw("Horizontal");
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