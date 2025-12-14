using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPickup : Interactable
{
    public override void Interact()
    {
        PlayerKeyHolder holder = FindObjectOfType<PlayerKeyHolder>();
        if (holder == null) return;

        holder.AttachKey(gameObject);
    }
}
