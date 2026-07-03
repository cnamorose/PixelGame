using UnityEngine;

public class PartItem : MonoBehaviour
{
    [Header("효과음")]
    public AudioClip itemSFX;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager_KM.Instance != null)
            {
                GameManager_KM.Instance.AddPart();
            }

            // 효과음 재생
            if (AudioManager.Instance != null && itemSFX != null)
            {
                AudioManager.Instance.PlaySFX(itemSFX);
            }

            // 부품 파괴
            Destroy(gameObject);
        }
    }
}