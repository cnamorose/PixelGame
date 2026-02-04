using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Game/PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("Player Info")]
    public string playerName = "플레이어";

    [Header("Progress")]
    public bool quizCleared = false;
    public bool pcCleared = false;
    public bool paperclear = false;

    [Header("Inventory")]
    public bool hasPen;
    public bool hasPaper;
    public bool hasUsb;
    public bool hasPaper2;

    [Header("Paper")]
    public bool paperTried = false;
    public void ResetForNewGame()
    {
        quizCleared = false;
        pcCleared = false;
        paperclear = false;

        hasPen = false;
        hasPaper = false;
        hasUsb = false;
        hasPaper2 = false;

        paperTried = false;
    }
}