using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager_KM : MonoBehaviour
{
    public static GameManager_KM Instance;

    public int partCount = 0;
    public int requiredParts = 1;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public bool HasAllParts()
    {
        return partCount >= requiredParts;
    }
}
