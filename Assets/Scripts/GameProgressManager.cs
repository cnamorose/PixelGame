using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance;

    [Header("¾Ç¸¶ ´ëÈ­¾À 1¹ø ¿©ºÎ")]
    public bool hasSeenFirstDialogue = false;

    [Header("¾Ç¸¶ ´ëÈ­¾À 2¹ø ¿©ºÎ")]
    public bool hasSeenPhase1EndDialogue = false;

    private void Awake()
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