using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource bgmSource;
    public AudioSource sfxSource;
    public AudioSource ambientSource;

    public AudioSource loopSFXSource;      // 발소리용
    public AudioSource oneShotSFXSource;   // 점프/대사용

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ---------------- BGM ----------------

    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource.clip == clip && bgmSource.isPlaying)
            return;

        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.volume = 1f;
        bgmSource.Play();
    }

    public void FadeOutBGM(float duration)
    {
        StartCoroutine(FadeOutCoroutine(duration));
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        float startVolume = bgmSource.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        bgmSource.volume = 0f;
        bgmSource.Stop();
    }

    public void FadeOutAndLoad(string sceneName, float duration)
    {
        StartCoroutine(FadeOutAndLoadCoroutine(sceneName, duration));
    }

    private IEnumerator FadeOutAndLoadCoroutine(string sceneName, float duration)
    {
        yield return FadeOutCoroutine(duration);
        SceneManager.LoadScene(sceneName);
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void FadeOutThenLoadScene(string sceneName, float duration)
    {
        StartCoroutine(FadeOutThenLoadSceneCoroutine(sceneName, duration));
    }

    private IEnumerator FadeOutThenLoadSceneCoroutine(string sceneName, float duration)
    {
        float startVolume = bgmSource.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        bgmSource.volume = 0f;
        bgmSource.Stop();

        SceneManager.LoadScene(sceneName);
    }

    // ---------------- Ambient ----------------

    public void PlayAmbient(AudioClip clip, float volume = 0.1f)
    {
        ambientSource.clip = clip;
        ambientSource.loop = true;
        ambientSource.volume = volume;
        ambientSource.Play();
    }

    public void StopAmbient()
    {
        ambientSource.Stop();
    }

    // ---------------- SFX ----------------

    public void PlaySFX(AudioClip clip, float pitchMin = 0.95f, float pitchMax = 1.05f)
    {
        sfxSource.pitch = Random.Range(pitchMin, pitchMax);
        sfxSource.PlayOneShot(clip);
    }

    public void PlayLoopingSFX(AudioClip clip)
    {
        if (loopSFXSource.clip == clip && loopSFXSource.isPlaying)
            return;

        loopSFXSource.clip = clip;
        loopSFXSource.loop = true;
        loopSFXSource.Play();
    }

    public void StopLoopingSFX()
    {
        loopSFXSource.Stop();
    }

    public void PlayOneShotSFX(AudioClip clip, float pitchMin = 0.95f, float pitchMax = 1.05f)
    {
        oneShotSFXSource.pitch = Random.Range(pitchMin, pitchMax);
        oneShotSFXSource.PlayOneShot(clip);
    }

    public void PlayOneShotWithPitch(AudioClip clip, float volume, float pitch)
    {
        if (clip == null) return;

        oneShotSFXSource.pitch = pitch;
        oneShotSFXSource.PlayOneShot(clip, volume);

        StartCoroutine(ResetPitch());
    }

    private IEnumerator ResetPitch()
    {
        yield return null; 
        oneShotSFXSource.pitch = 1f;
    }
}
