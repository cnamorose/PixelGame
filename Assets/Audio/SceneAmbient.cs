using UnityEngine;

public class SceneAmbient : MonoBehaviour
{
    public AudioClip ambientClip;

    void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayAmbient(ambientClip, 1.0f);
        }
    }
}
