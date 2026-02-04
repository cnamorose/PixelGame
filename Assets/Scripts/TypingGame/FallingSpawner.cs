using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FallingSpawner : MonoBehaviour
{
    public GameObject wordPrefab;
    public Transform lettersParent;
    public TypingGameManager manager;
    public RectTransform spawnArea;

    public float spawnInterval = 1.0f;
    private float timer;

    public string[] wordList =
    {
        "apple",
        "unity",
        "pixel",
        "typing",
        "acid"
    };

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnWord();
            timer = 0f;
        }
    }

    void SpawnWord()
    {
        GameObject obj = Instantiate(wordPrefab, lettersParent);

        RectTransform wordRT = obj.GetComponent<RectTransform>();
        RectTransform areaRT = lettersParent.GetComponent<RectTransform>();

        float areaWidth = areaRT.rect.width;

        // X는 TypingArea 내부 랜덤
        float x = Random.Range(-areaWidth * 0.5f, areaWidth * 0.5f);

        float y = 0f;

        wordRT.anchoredPosition = new Vector2(x, y);

        string w = wordList[Random.Range(0, wordList.Length)];
        WordMovement wm = obj.GetComponent<WordMovement>();
        wm.SetWord(w);

        manager.RegisterWord(wm);
    }

}