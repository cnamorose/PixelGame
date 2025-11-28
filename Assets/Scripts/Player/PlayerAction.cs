using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerAction : MonoBehaviour
{
    public float Speed;
    Animator anim;

    Rigidbody2D rigid;
    float h;
    float v;
    bool isHorizonMove;
    bool isQuizScene = false;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
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
    }
}