using UnityEngine;

public class PartItem : MonoBehaviour
{
    [Header("È¿°úÀ½")]
    public AudioClip itemSFX; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager_KM.Instance.partCount++;

            if (AudioManager.Instance != null && itemSFX != null)
            {
                AudioManager.Instance.PlaySFX(itemSFX);
            }
            Destroy(gameObject);
        }
    }
}
