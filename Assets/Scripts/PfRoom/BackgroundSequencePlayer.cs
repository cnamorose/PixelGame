using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSequencePlayer : MonoBehaviour
{
    public SpriteRenderer sr;

    [Header("Sprites")]
    public Sprite[] sprites;     // 10장

    [Header("Timing")]
    public float frameTime = 0.2f;   // 한 장당 시간

    void Start()
    {
        if (sr == null)
            sr = GetComponent<SpriteRenderer>();

        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            sr.sprite = sprites[i];
            yield return new WaitForSeconds(frameTime);
        }
    }
}