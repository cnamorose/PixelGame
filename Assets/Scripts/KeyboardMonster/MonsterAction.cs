using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAction : MonoBehaviour
{
    public float speed = 1.5f;
    public Transform leftPoint;
    public Transform rightPoint;

    private bool movingRight = true;
    private float changeTime;
    private SpriteRenderer sr;

    public Transform player;
    public float soundRange = 4f;

    public AudioClip walkClip;      
    private AudioSource walkSource;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        walkSource = GetComponent<AudioSource>();

        if (walkSource != null)
        {
            walkSource.clip = walkClip;
            walkSource.loop = true;
            walkSource.playOnAwake = false;
        }

        GameObject playerObj = GameObject.FindWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("Player not found!");
        }

        SetRandomTime();
    }

    void Update()
    {
        if (movingRight)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector2.left * speed * Time.deltaTime);
        }

        sr.flipX = !movingRight;

        changeTime -= Time.deltaTime;
        if (changeTime <= 0)
        {
            movingRight = !movingRight;
            SetRandomTime();
        }

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

        CheckWalkSound();
    }

    void CheckWalkSound()
    {
        if (player == null || walkSource == null || walkClip == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= soundRange)
        {
            if (!walkSource.isPlaying)
                walkSource.Play();
        }
        else
        {
            if (walkSource.isPlaying)
                walkSource.Stop();
        }
    }

    void SetRandomTime()
    {
        changeTime = Random.Range(0.5f, 2f);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            //PlayerLifeManager.Instance.LoseLife();
            //Destroy(gameObject);
        }
    }
}