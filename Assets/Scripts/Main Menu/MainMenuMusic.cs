using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MenuMusicHandler : MonoBehaviour
{
    [Header("Music Settings")]
    public AudioClip menuSong;
    [Range(0f, 1f)] public float targetVolume = 0.5f;
    public float fadeDuration = 1.5f;

    private AudioSource audioSource;
    private bool isFading = false;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        audioSource.clip = menuSong;
        audioSource.volume = targetVolume;
        audioSource.loop = true;
        audioSource.Play();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex != 0 && !isFading)
        {
            StartCoroutine(FadeOutAndDestroy());
        }
    }

    private IEnumerator FadeOutAndDestroy()
    {
        isFading = true;
        float startVolume = audioSource.volume;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null; 
        }

        audioSource.volume = 0;
        audioSource.Stop();
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}