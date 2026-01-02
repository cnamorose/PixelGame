using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundLoop : MonoBehaviour
{
    public Sprite[] backgrounds;   // πË∞Ê 3¿Â
    public float changeInterval = 0.5f;

    private SpriteRenderer sr;
    private int index = 0;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = backgrounds[0];
        InvokeRepeating(nameof(ChangeBackground), changeInterval, changeInterval);
    }

    void ChangeBackground()
    {
        index = (index + 1) % backgrounds.Length;
        sr.sprite = backgrounds[index];
    }
}
