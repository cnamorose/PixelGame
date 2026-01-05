using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class EvenlySpaceChildren : MonoBehaviour
{
    public float spacing = 1.0f;
    public Vector2 startOffset = Vector2.zero;
    public bool alignX = true;
    public bool alignY = false;

    [ContextMenu("Align Children")]
    void Align()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            Vector3 pos = child.localPosition;

            if (alignX)
                pos.x = startOffset.x + spacing * i;

            if (alignY)
                pos.y = startOffset.y + spacing * i;

            child.localPosition = pos;
        }
    }
}
