using UnityEngine;

public class SceneBGM : MonoBehaviour
{
    public AudioClip sceneBGM;

    void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBGM(sceneBGM);
        }
    }
}