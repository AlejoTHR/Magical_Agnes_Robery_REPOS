using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections; // Required for Coroutines

public class MainMenu : MonoBehaviour
{
    [Header("Canvases")]
    public GameObject _MainPanel;
    public GameObject _OptionsPanel;

    [Header("Controller Navigation")]
    public GameObject _FirstButtonMain;
    public GameObject _FirstButtonOptions;

    [Header("Sound")]
    public AudioMixer _AudioMixer;
    public Slider _MusicSlider;

    [Header("Transitions")]
    [SerializeField] private Animator _transitionAnimator;
    [SerializeField] private string _hideScreenAnim = "FadeIn";
    [SerializeField] private string _showScreenAnim = "FadeOut";
    [SerializeField] private float _animDuration = 1.0f;

    private bool isTransitioning = false;

    private void Start()
    {
        _MainPanel.SetActive(true);
        _OptionsPanel.SetActive(false);

        if (PlayerPrefs.HasKey("musicVolume")) LoadVolume();
        else { _MusicSlider.value = 0.75f; SetMusicVolume(); }

        FocusButton(_FirstButtonMain);

        // Play the FadeOut (reveal) when the menu first opens
        if (_transitionAnimator != null)
        {
            _transitionAnimator.Play(_showScreenAnim);
        }
    }

    public void StartGame()
    {
        if (isTransitioning) return;
        StartCoroutine(StartGameSequence());
    }

    private IEnumerator StartGameSequence()
    {
        isTransitioning = true;

        if (_transitionAnimator != null)
        {
            _transitionAnimator.Play(_hideScreenAnim);
        }

        yield return new WaitForSeconds(_animDuration);

        // Tell the LevelManager in the Tutorial scene to play its FadeOut
        LevelManager._shouldFadeOutOnArrival = true;

        SceneManager.LoadScene("0 - Tutorial");
    }

    // --- EXISTING LOGIC ---
    public void SetMusicVolume()
    {
        float volume = _MusicSlider.value;
        float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;
        _AudioMixer.SetFloat("music", dB);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    public void LoadVolume()
    {
        float savedVolume = PlayerPrefs.GetFloat("musicVolume");
        _MusicSlider.value = savedVolume;
        SetMusicVolume();
    }

    public void CloseGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void OpenOptions()
    {
        _MainPanel.SetActive(false);
        _OptionsPanel.SetActive(true);
        FocusButton(_FirstButtonOptions);
    }

    public void Back()
    {
        _OptionsPanel.SetActive(false);
        _MainPanel.SetActive(true);
        FocusButton(_FirstButtonMain);
    }

    private void FocusButton(GameObject target)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(target);
    }
}