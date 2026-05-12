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
        // Keeps the music playing into the next scene so it can fade out
        DontDestroyOnLoad(gameObject);
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        audioSource.clip = menuSong;
        audioSource.volume = targetVolume;
        audioSource.loop = true;
        audioSource.Play();

        // Subscribe to the scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Called automatically whenever a new scene is loaded
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // If we are no longer in the Main Menu (index 0), start fading
        // Change "0" to your actual Main Menu scene build index if different
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
            yield return null; // Wait for next frame
        }

        audioSource.volume = 0;
        audioSource.Stop();

        // Clean up the object so it doesn't leak memory or stay in the game
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}