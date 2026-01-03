using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DevilVisual : MonoBehaviour
{
    Image img;

    void Awake()
    {
        img = GetComponent<Image>();
        img.enabled = false;
    }

    public void Show()
    {
        img.enabled = true;
    }

    public void SetSprite(Sprite sprite)
    {
        img.sprite = sprite;
    }
}

