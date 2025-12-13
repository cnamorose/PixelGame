using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bookshelf : Interactable
{
    public override void Interact()
    {
        SceneManager.LoadScene("Quiz");
    }
}
