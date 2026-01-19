using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DisappearingBlocks : MonoBehaviour
{
    public List<GameObject> blocks;
    public float interval = 2f;

    public enum Mode
    {
        Normal, 
        Two    
    }

    [Header("Block Mode")]
    public Mode mode;

    void Start()
    {
        switch (mode)
        {
            case Mode.Normal:
                StartCoroutine(NormalSequence());
                break;

            case Mode.Two:
                StartCoroutine(TwoBlockSequence());
                break;
        }
    }
    IEnumerator NormalSequence()
    {
        while (true)
        {
            foreach (GameObject block in blocks)
            {
                block.SetActive(false);
                yield return new WaitForSeconds(interval);
            }

            foreach (GameObject block in blocks)
            {
                block.SetActive(true);
                yield return new WaitForSeconds(interval);
            }
        }
    }

    IEnumerator TwoBlockSequence()
    {
        Queue<GameObject> activeBlocks = new Queue<GameObject>();

        foreach (GameObject block in blocks)
            block.SetActive(false);

        int index = 0;

        while (true)
        {
            GameObject block = blocks[index];
            block.SetActive(true);
            activeBlocks.Enqueue(block);

            if (activeBlocks.Count > 2)
            {
                GameObject oldBlock = activeBlocks.Dequeue();
                oldBlock.SetActive(false);
            }

            index = (index + 1) % blocks.Count;
            yield return new WaitForSeconds(interval);
        }
    }
}