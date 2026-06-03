using UnityEngine;

public class MenuAudioManager : MonoBehaviour
{
    public static MenuAudioManager Instance;
    private AudioSource source;
    private float lastClickTime;
    private float clickProtectionDuration = 0.25f; 

    void Awake()
    {
        if (Instance == null) Instance = this;
        source = GetComponent<AudioSource>();
    }

    public void PlayMenuSound(AudioClip clip, bool isClick)
    {
        if (clip == null) return;
        if (!isClick && Time.unscaledTime < lastClickTime + clickProtectionDuration)
        {
            return;
        }

        if (isClick)
        {
            lastClickTime = Time.unscaledTime;
        }

        source.Stop();
        source.clip = clip;
        source.Play();
    }
}