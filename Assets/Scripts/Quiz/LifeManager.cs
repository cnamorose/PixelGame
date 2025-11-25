using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeManager : MonoBehaviour
{
    public Image[] pills;     
    public Sprite fullPill;   
    public Sprite emptyPill;  

    private int life = 3;

    public void LoseLife()
    {
        if (life <= 0) return;

        life--;

        // ±ôºý
        StartCoroutine(BlinkPill(pills[life]));
    }

    private IEnumerator BlinkPill(Image pill)
    {
        for (int i = 0; i < 3; i++)
        {
            pill.enabled = false;
            yield return new WaitForSeconds(0.1f);

            pill.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }

        pill.sprite = emptyPill;
    }
}
