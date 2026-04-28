using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Standard UI for Sliders
using UnityEngine.EventSystems; // Required for Controller navigation

public class MainMenu : MonoBehaviour
{
    [Header("Canvases")]
    public GameObject _MainPanel;    // Use GameObjects instead of Canvas for easier control
    public GameObject _OptionsPanel;

    [Header("Controller Navigation")]
    public GameObject _FirstButtonMain;    // Assign your 'Start' button
    public GameObject _FirstButtonOptions; // Assign your 'Music Slider' or 'Back' button

    [Header("Sound")]
    public AudioMixer _AudioMixer;
    public Slider _MusicSlider;

    private void Start()
    {
        // Initializing the Menu state
        _MainPanel.SetActive(true);
        _OptionsPanel.SetActive(false);

        // Load Volume or set default
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            LoadVolume();
        }
        else
        {
            // Default to 0.75 if no save exists
            _MusicSlider.value = 0.75f;
            SetMusicVolume();
        }

        // Focus the first button for controller support
        FocusButton(_FirstButtonMain);
    }

    // --- MUSIC LOGIC ---
    public void SetMusicVolume()
    {
        float volume = _MusicSlider.value;
        // Clamp value to avoid Log10(0) which is undefined
        float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;

        _AudioMixer.SetFloat("music", dB);
        PlayerPrefs.SetFloat("musicVolume", volume); // Save the 0-1 value for the slider
    }

    public void LoadVolume()
    {
        float savedVolume = PlayerPrefs.GetFloat("musicVolume");
        _MusicSlider.value = savedVolume;
        SetMusicVolume();
    }

    // --- NAVIGATION LOGIC ---
    public void StartGame()
    {
        SceneManager.LoadScene("0 - Tutorial");
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

    // Helper method to ensure the controller always has a "Selected" object
    private void FocusButton(GameObject target)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(target);
    }
}