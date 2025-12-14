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

    public bool hasPen;
    public bool hasPaper;
    public bool hasUsb;
    public bool hasPaper2;
}
