using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryState : MonoBehaviour
{
    public static StoryState Instance;

    public bool quizCleared = false;
    public bool pcStageCleared = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
