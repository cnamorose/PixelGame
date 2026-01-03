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




    public void LockControl()
    {
        forceIdle = true;
        rigid.velocity = Vector2.zero;
    }

    public void UnlockControl()
    {
        forceIdle = false;
    }

    public enum PlayerMoveMode
    {
        TopDown,
        Platformer
    }
    void OnTriggerEnter2D(Collider2D other)
    {
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
        if (GameOverManager.Instance != null &&
        GameOverManager.Instance.isGameOverSequenceRunning)
            return;

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
            SceneManager.LoadScene("DevilMonster");
            return;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Room");
            return;
        }

        if (isDevilMonsterScene)
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

            return; // ⭐ Animator 로직 완전 차단
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

        // 애니메이션 갱신 (기존 코드 그대로 유지)
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
    }

    private void FixedUpdate()
    {
        if (forceIdle || isQuizScene || isInventoryOpen)
        {
            rigid.velocity = Vector2.zero;
            return;
        }

        // 바닥 체크 (Platformer 모드에서만 사용)
        if (moveMode == PlayerMoveMode.Platformer)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.25f, groundLayer);

            // 플랫폼 이동: 좌우만 움직임, Y속도는 중력 유지
            rigid.velocity = new Vector2(h * Speed, rigid.velocity.y);
        }
        else
        {
            // 기존 탑다운 이동 형태 그대로 유지
            Vector2 moveVec = isHorizonMove ? new Vector2(h, 0) : new Vector2(0, v);
            rigid.velocity = moveVec * Speed;
        }
    }

    void HandleTopDownInput()
    {
        // 기존 방식 그대로
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        // 버튼 체크 → isHorizonMove 유지
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
        // 좌우만 입력 가능
        h = Input.GetAxisRaw("Horizontal");
        v = 0;

        // 점프
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            Jump();
    }

    void Jump()
    {
        rigid.velocity = new Vector2(rigid.velocity.x, jumpForce);
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

        // =========================
        // 🎮 Quiz 씬 처리 (기존 유지)
        // =========================
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

        // =========================
        // 🕹 이동 모드 전환 (중요!!!)
        // =========================
        if (scene.name == "KeyboardMonster" || scene.name == "Keyboard_boss")
        {
            moveMode = PlayerMoveMode.Platformer;
            rigid.gravityScale = 1f;
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
        else
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

            // ⭐ 항상 옆모습으로 시작
            idleDir = 1;          // 기본 오른쪽
            sr.flipX = false;

            // ⭐ Animator를 사이드 상태로 강제
            anim.enabled = true;
            anim.speed = 0f;                      // 애니 정지
            anim.Play("Player_R", 0, 0f);          // 사이드 걷기 첫 프레임
            anim.Update(0f);                       // 즉시 반영

            // 파라미터 정리 (기존 유지)
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

            // 🔒 기존 초기화 유지
            idleDir = 1;
            sr.flipX = false;
            anim.SetInteger("hAxisRaw", 1);
            anim.SetInteger("vAxisRaw", 0);
            anim.SetBool("isChange", false);
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
        yield return null; // 한 프레임
        anim.SetBool("isChange", false);
    }

}
