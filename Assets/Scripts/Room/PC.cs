using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PC : Interactable
{
    public Sprite normalSprite;
    public Sprite activeSprite;

    SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        if (StoryState.Instance.quizCleared)
            sr.sprite = activeSprite;
    }

    public override void Interact()
    {
        if (!StoryState.Instance.quizCleared)
        {
            DialogueManager.Instance.ShowSimpleDialogue(
                "다른 스테이지를 클리어해야 한다"
            );
            return;
        }

        SceneManager.LoadScene("PCStage");
    }
}
