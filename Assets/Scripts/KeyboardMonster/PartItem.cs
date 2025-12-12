using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartItem : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager_KM.Instance.partCount++;
            Destroy(gameObject);
        }
    }
}
