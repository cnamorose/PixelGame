using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAction: MonoBehaviour
{
    public float speed = 1.5f;
    public Transform leftPoint;
    public Transform rightPoint;

    private bool movingRight = true;
    private float changeTime;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        SetRandomTime();
    }

    void Update()
    {
        // 이동
        if (movingRight)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector2.left * speed * Time.deltaTime);
        }

        // 방향 반전
        sr.flipX = !movingRight;

        // 랜덤 시간마다 방향 전환
        changeTime -= Time.deltaTime;
        if (changeTime <= 0)
        {
            movingRight = !movingRight;
            SetRandomTime();
        }

        // 포인트 범위를 넘지 않도록 체크
        if (transform.position.x > rightPoint.position.x)
        {
            movingRight = false;
            SetRandomTime();
        }

        if (transform.position.x < leftPoint.position.x)
        {
            movingRight = true;
            SetRandomTime();
        }

        float x = transform.position.x;
        x = Mathf.Clamp(x, leftPoint.position.x, rightPoint.position.x);
        transform.position = new Vector2(x, transform.position.y);
    }

    void SetRandomTime()
    {
        changeTime = Random.Range(0.5f, 2f);   // 0.5~2초 사이로 랜덤하게 방향 바꿈
    }
}