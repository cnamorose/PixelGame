using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyItem : MonoBehaviour
{
    bool isAttached = false;
    Vector3 baseLocalPos;
    Vector3 baseWorldPos;

    void Start()
    {
        baseWorldPos = transform.position;
    }

    public void OnAttachedToPlayer()
    {
        isAttached = true;
        baseLocalPos = transform.localPosition;
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * 4f) * 0.1f;

        if (isAttached)
        {
            transform.localPosition = baseLocalPos + Vector3.up * offset;
        }
        else
        {
            transform.position = baseWorldPos + Vector3.up * offset;
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("¿­¼è Æ®¸®°Å: " + other.name);

        PlayerKeyHolder holder = other.GetComponent<PlayerKeyHolder>();
        Debug.Log("holder = " + holder);

        if (holder == null) return;
        holder.AttachKey(gameObject);
    }
}