using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKeyHolder : MonoBehaviour
{
    public Transform keyAnchor;
    GameObject currentKey;

    public void AttachKey(GameObject key)
    {
        if (currentKey != null) return;

        currentKey = key;

        key.transform.SetParent(keyAnchor);
        key.transform.localPosition = Vector3.zero;
        key.transform.localRotation = Quaternion.identity;

        key.GetComponent<KeyItem>()?.OnAttachedToPlayer();

        // ⭐ 물리 완전 제거 (중요)
        Rigidbody2D rb = key.GetComponent<Rigidbody2D>();
        if (rb != null) Destroy(rb);

        Collider2D col = key.GetComponent<Collider2D>();
        if (col != null) Destroy(col);
    }

    public bool HasKey()
    {
        return currentKey != null;
    }

    public void UseKey()
    {
        if (currentKey != null)
        {
            Destroy(currentKey);
            currentKey = null;
        }
    }
}