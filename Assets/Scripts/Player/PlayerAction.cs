using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerAction : MonoBehaviour
{
    public AnimatorOverrideController PlayerM;
    public RuntimeAnimatorController Player;

    public float Speed;
    Animator anim;

    Rigidbody2D rigid;
    float h;
    float v;
    bool isHorizonMove;
    bool isQuizScene = false;

    public void SetCharacter(string type)
    {
        if (anim == null)
            anim = GetComponent<Animator>();

        // 기본 스프라이트 제거
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
        Debug.Log("Player Awake! instance id = " + GetInstanceID());

    }

    void Start()
    {
        string character = PlayerPrefs.GetString("SelectedCharacter", "Girl");

        // 스프라이트 초기화 (기본 SpriteRenderer 값 제거)
        GetComponent<SpriteRenderer>().sprite = null;

        // 애니메이터 적용
        if (character == "Boy")
            anim.runtimeAnimatorController = PlayerM;
        else
            anim.runtimeAnimatorController = Player;

        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    void Update()
    {

        if (isQuizScene)
            return;

        //Move Value
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        //Check Button Down & Up
        bool hDown = Input.GetButtonDown("Horizontal");
        bool vDown = Input.GetButtonDown("Vertical");
        bool hUp = Input.GetButtonUp("Horizontal");
        bool vUp = Input.GetButtonUp("Vertical");

        //Check Horizontal Move
        if (hDown)
            isHorizonMove = true;
        else if (vDown)
            isHorizonMove = false;
        else if ( hUp || vUp)
            isHorizonMove = h != 0;

        //Animation
        if (anim.GetInteger("hAxisRaw") != h)
        {
            anim.SetBool("isChange", true);
            anim.SetInteger("hAxisRaw", (int)h);
        }
        else if(anim.GetInteger("vAxisRaw") != v)
        {
            anim.SetBool("isChange", true);
            anim.SetInteger("vAxisRaw", (int)v);
        }
        else
            anim.SetBool("isChange", false);


        if (Input.GetKeyDown(KeyCode.Q))
        {
            SceneManager.LoadScene("Quiz");
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            SceneManager.LoadScene("Room");
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            SceneManager.LoadScene("school_1");
        }
    }

    private void FixedUpdate()
    {
        if (isQuizScene)
        {
            rigid.velocity = Vector2.zero;
            return;
        }

        //Move
        Vector2 moveVec = isHorizonMove ? new Vector2(h, 0) : new Vector2(0, v);
        rigid.velocity = moveVec * Speed;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded 실행됨. 씬 = " + scene.name);

        if (scene.name == "Quiz")
        {
            // 플레이어 숨기기 + 움직임 차단
            isQuizScene = true;
            GetComponent<SpriteRenderer>().enabled = false;
            rigid.velocity = Vector2.zero;
        }
        else
        {
            // 다시 보이게 + 움직임 가능
            isQuizScene = false;
            GetComponent<SpriteRenderer>().enabled = true;
        }

        GameObject spawn = GameObject.Find("PlayerPoint");
        if (spawn != null)
        {
            transform.position = spawn.transform.position;
            Debug.Log("spawn 결과 = " + spawn);
        }

    }
}