using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearingBlocks : MonoBehaviour
{
    public List<GameObject> blocks;   // 사라질 블럭들
    public float interval = 2f;       // 각 블럭 간 시간 간격

    void Start()
    {
        StartCoroutine(BlockSequence());
    }

    IEnumerator BlockSequence()
    {
        while (true)
        {
            // 하나씩 순서대로 사라짐
            foreach (GameObject block in blocks)
            {
                block.SetActive(false);
                yield return new WaitForSeconds(interval);
            }

            // 다시 하나씩 순서대로 나타남
            foreach (GameObject block in blocks)
            {
                block.SetActive(true);
                yield return new WaitForSeconds(interval);
            }
        }
    }
}
