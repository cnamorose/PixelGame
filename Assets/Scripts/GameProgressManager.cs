using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance;

    [Header("악마 대화씬 1번 여부")]
    public bool hasSeenFirstDialogue = false;

    [Header("악마 대화씬 2번 여부")]
    public bool hasSeenPhase1EndDialogue = false;

    [Header("보스 인트로 대화 여부")]
    public bool hasSeenBossIntro = false;

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