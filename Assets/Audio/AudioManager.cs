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

    // 이 함수를 찾아서 아래 내용으로 바꿔주세요!
    public void PlayBGM(AudioClip clip, bool isLoop = true)
    {
        if (bgmSource.clip == clip && bgmSource.isPlaying)
            return;

        bgmSource.clip = clip;
        bgmSource.loop = isLoop; // ◀ 강제 true 대신 전달받은 값(isLoop)이 들어갑니다.
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

    public void StopAllSFX()
    {
        if (sfxSource != null) sfxSource.Stop();
        if (loopSFXSource != null) loopSFXSource.Stop();
        if (oneShotSFXSource != null) oneShotSFXSource.Stop();
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

    // ---------------- 볼륨 제어 기능 추가 ----------------

    // BGM 볼륨 조절 (0.0f ~ 1.0f)
    public void SetBGMVolume(float volume)
    {
        if (bgmSource != null)
        {
            bgmSource.volume = volume;
        }
    }

    // 효과음 볼륨 조절 (모든 SFX 관련 소스에 일괄 적용)
    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null) sfxSource.volume = volume;
        if (loopSFXSource != null) loopSFXSource.volume = volume;
        if (oneShotSFXSource != null) oneShotSFXSource.volume = volume;
    }

    // 환경음 볼륨 조절
    public void SetAmbientVolume(float volume)
    {
        if (ambientSource != null)
        {
            ambientSource.volume = volume;
        }
    }

}
