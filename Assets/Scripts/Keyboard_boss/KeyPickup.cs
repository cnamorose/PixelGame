using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public float cameraReleaseDelay = 1f;
    bool picked = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (picked) return;

        PlayerKeyHolder holder = other.GetComponent<PlayerKeyHolder>();
        if (holder == null) return;

        picked = true;

        holder.AttachKey(gameObject);

        Cameramove cam = Camera.main?.GetComponent<Cameramove>();
        if (cam != null)
            StartCoroutine(ReleaseCameraAfterDelay(cam));
    }

    IEnumerator ReleaseCameraAfterDelay(Cameramove cam)
    {
        yield return new WaitForSeconds(cameraReleaseDelay);

        if (cam != null)
            cam.ForceResetToPlayer();

        GameObject p = GameObject.FindWithTag("Player");
        if (p != null)
        {
            PlayerAction pa = p.GetComponent<PlayerAction>();
            if (pa != null)
                pa.limitByCamera = false;
        }
    }
}