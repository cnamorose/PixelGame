using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevilVisual : MonoBehaviour
{
    SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void SetSprite(Sprite sprite)
    {
        sr.sprite = sprite;
    }
}

