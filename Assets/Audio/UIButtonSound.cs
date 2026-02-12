using UnityEngine;

public class UIButtonSound : MonoBehaviour
{
    public AudioClip clickSound;

    public void PlayClick()
    {
        if (AudioManager.Instance != null && clickSound != null)
        {
            AudioManager.Instance.PlaySFX(clickSound);
        }
    }
}