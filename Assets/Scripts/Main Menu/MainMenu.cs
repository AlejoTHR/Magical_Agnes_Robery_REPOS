using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance;

    [Header("Canvases")]
    public GameObject _MainPanel;
    public GameObject _OptionsPanel;
    public GameObject _AudioPanel;
    public GameObject _CreditsPanel;

    [Header("Controller Navigation")]
    public GameObject _FirstButtonMain;
    public GameObject _FirstButtonOptions;
    public GameObject _FirstButtonAudio;
    public GameObject _FirstButtonCredits;

    [Header("Sound Settings")]
    public AudioMixer _AudioMixer;
    public Slider _MusicSlider;
    public Slider _Mainslider;
    public Slider _SFXSlider;
    public AudioClip _globalClickSound;

    private AudioSource _hoverChannel;
    private AudioSource _clickChannel;

    [Header("Transitions")]
    [SerializeField] private Animator _transitionAnimator;
    [SerializeField] private string _hideScreenAnim = "FadeIn";
    [SerializeField] private string _showScreenAnim = "FadeOut";
    [SerializeField] private float _animDuration = 1.0f;

    private bool isTransitioning = false;

    private void Awake()
    {
        Instance = this;
        _hoverChannel = gameObject.AddComponent<AudioSource>();
        _clickChannel = gameObject.AddComponent<AudioSource>();
        _clickChannel.priority = 0;
        _hoverChannel.playOnAwake = false;
        _clickChannel.playOnAwake = false;
    }

    private void Start()
    {
        _MainPanel.SetActive(true);
        _OptionsPanel.SetActive(false);
        _AudioPanel.SetActive(false);
        _CreditsPanel.SetActive(false);

        LoadMasterVolume();
        LoadSFXVolume();
        LoadVolume();

        if (_Mainslider != null) _Mainslider.onValueChanged.AddListener(delegate { SetMasterVolume(); });
        if (_MusicSlider != null) _MusicSlider.onValueChanged.AddListener(delegate { SetMusicVolume(); });
        if (_SFXSlider != null) _SFXSlider.onValueChanged.AddListener(delegate { SetSFXVolume(); });

        FocusButton(_FirstButtonMain);

        if (_transitionAnimator != null) _transitionAnimator.Play(_showScreenAnim);
    }

    public void StartGame()
    {
        if (isTransitioning) return;
        UI_PlayClick();
        StartCoroutine(StartGameSequence());
    }

    private IEnumerator StartGameSequence()
    {
        isTransitioning = true;

        if (_transitionAnimator != null) _transitionAnimator.Play(_hideScreenAnim);

        yield return new WaitForSeconds(_animDuration);

        SceneManager.LoadScene("0 - Tutorial");
    }

    public void OpenOptions()
    {
        UI_PlayClick();
        _MainPanel.SetActive(false);
        _OptionsPanel.SetActive(true);
        FocusButton(_FirstButtonOptions);
    }

    public void BackFromOptions()
    {
        UI_PlayClick();
        _OptionsPanel.SetActive(false);
        _MainPanel.SetActive(true);
        FocusButton(_FirstButtonMain);
    }

    public void OpenCredits()
    {
        UI_PlayClick();
        _OptionsPanel.SetActive(false);
        _CreditsPanel.SetActive(true);
        FocusButton(_FirstButtonCredits);
    }

    public void BackFromCredits()
    {
        UI_PlayClick();
        _CreditsPanel.SetActive(false);
        _OptionsPanel.SetActive(true);
        FocusButton(_FirstButtonOptions);
    }

    public void OpenAudio()
    {
        UI_PlayClick();
        _OptionsPanel.SetActive(false);
        _AudioPanel.SetActive(true);
        FocusButton(_FirstButtonAudio);
    }

    public void BackFromAudio()
    {
        UI_PlayClick();
        _AudioPanel.SetActive(false);
        _OptionsPanel.SetActive(true);
        FocusButton(_FirstButtonOptions);
    }

    public void CloseGame()
    {
        UI_PlayClick();
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }


    public void PlayHoverSound(AudioClip clip)
    {
        if (clip == null || isTransitioning || _clickChannel.isPlaying) return;
        _hoverChannel.Stop();
        _hoverChannel.clip = clip;
        _hoverChannel.Play();
    }

    public void UI_PlayClick()
    {
        if (_globalClickSound == null) return;
        _hoverChannel.Stop();
        _clickChannel.Stop();
        _clickChannel.clip = _globalClickSound;
        _clickChannel.Play();

        StartCoroutine(StopAudioAfterDelay(0.4f));
    }

    private IEnumerator StopAudioAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _clickChannel.Stop();
    }

    public void SetMusicVolume()
    {
        if (_MusicSlider == null || _AudioMixer == null) return;
        float dB = Mathf.Log10(Mathf.Clamp(_MusicSlider.value, 0.0001f, 1f)) * 20;
        _AudioMixer.SetFloat("music", dB);
        PlayerPrefs.SetFloat("musicVolume", _MusicSlider.value);
    }

    public void SetSFXVolume()
    {
        if (_SFXSlider == null || _AudioMixer == null) return;
        float dB = Mathf.Log10(Mathf.Clamp(_SFXSlider.value, 0.0001f, 1f)) * 20;
        _AudioMixer.SetFloat("SFX", dB);
        PlayerPrefs.SetFloat("sfxVolume", _SFXSlider.value);
    }

    public void SetMasterVolume()
    {
        if (_Mainslider == null || _AudioMixer == null) return;
        float dB = Mathf.Log10(Mathf.Clamp(_Mainslider.value, 0.0001f, 1f)) * 20;
        _AudioMixer.SetFloat("Master", dB);
        PlayerPrefs.SetFloat("masterVolume", _Mainslider.value);
    }

    public void LoadVolume()
    {
        if (_MusicSlider == null) return;
        _MusicSlider.value = PlayerPrefs.GetFloat("musicVolume", 0.75f);
        SetMusicVolume(); 
    }

    public void LoadMasterVolume()
    {
        if (_Mainslider == null) return;
        _Mainslider.value = PlayerPrefs.GetFloat("masterVolume", 0.75f);
        SetMasterVolume(); 
    }

    public void LoadSFXVolume()
    {
        if (_SFXSlider == null) return;
        _SFXSlider.value = PlayerPrefs.GetFloat("sfxVolume", 0.75f);
        SetSFXVolume(); 
    }

    private void FocusButton(GameObject target)
    {
        if (target == null) return;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(target);
    }
}