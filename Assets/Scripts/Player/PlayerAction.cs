using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerAction : MonoBehaviour
{
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
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Update()
    {
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
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SceneManager.LoadScene("Quiz");
            return;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            SceneManager.LoadScene("KeyboardMonster");
            return;
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
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

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
            inventoryUI.SetActive(false); // 여기서 숨김
            isInventoryOpen = false;
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

        GameObject spawn = GameObject.Find("PlayerPoint");
        if (spawn != null)
            transform.position = spawn.transform.position;

        if (scene.name == "KeyboardMonster" || scene.name == "Keyboard_boss")
        {
            if (scene.name == "KeyboardMonster")
            {
                moveMode = PlayerMoveMode.Platformer;
                rigid.gravityScale = 1f;
                transform.localScale = new Vector3(
                transform.localScale.x * 0.5f,
                transform.localScale.y * 0.5f,
                transform.localScale.z
                );
            }
            if (scene.name == "Keyboard_boss")
            {
                moveMode = PlayerMoveMode.Platformer;
                rigid.gravityScale = 1f;
            }
        }
        else
        {
            moveMode = PlayerMoveMode.TopDown;
            rigid.gravityScale = 0f;
            rigid.velocity = Vector2.zero;

        }
    }
}
