using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneRunner : MonoBehaviour
{
    public float runSpeed = 4f;
    public Sprite[] runSprites;
    public float animInterval = 0.15f;

    SpriteRenderer sr;
    int spriteIndex = 0;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        StartCoroutine(RunAnim());
    }

    void Update()
    {
        transform.Translate(Vector2.right * runSpeed * Time.deltaTime);
    }

    System.Collections.IEnumerator RunAnim()
    {
        while (true)
        {
            sr.sprite = runSprites[spriteIndex];
            spriteIndex = (spriteIndex + 1) % runSprites.Length;
            yield return new WaitForSeconds(animInterval);
        }
    }
}