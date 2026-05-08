using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Collections;

public class MenuPausa : MonoBehaviour
{
    public static MenuPausa Instance;

    [Header("Panels")]
    public GameObject panelPausa;
    public GameObject controlsMenu;

    [Header("Sound Settings")]
    public AudioMixer mainMixer;
    public Slider musicSlider;
    public AudioClip _globalClickSound;

    private AudioSource _hoverChannel;
    private AudioSource _clickChannel;

    [Header("Controller Setup")]
    public GameObject firstButtonPause;
    public GameObject firstButtonControls;

    public bool juegoPausado;

    void Awake()
    {
        Instance = this;

        // Setup channels that ignore Time.timeScale = 0
        _hoverChannel = gameObject.AddComponent<AudioSource>();
        _clickChannel = gameObject.AddComponent<AudioSource>();

        _hoverChannel.ignoreListenerPause = true;
        _clickChannel.ignoreListenerPause = true;
        _clickChannel.priority = 0;
        _hoverChannel.playOnAwake = false;
        _clickChannel.playOnAwake = false;
    }

    void Start()
    {
        float currentVol;
        if (mainMixer.GetFloat("music", out currentVol))
        {
            musicSlider.value = Mathf.Pow(10, currentVol / 20);
        }
    }

    // --- AUDIO HUB ---

    public void PlayHoverSound(AudioClip clip)
    {
        if (clip == null || _clickChannel.isPlaying) return;
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

        // Cut off audio after 0.4 seconds
        CancelInvoke(nameof(StopClickAudio));
        Invoke(nameof(StopClickAudio), 0.4f);
    }

    private void StopClickAudio() => _clickChannel.Stop();

    // --- PAUSE LOGIC ---

    public void OnTogglePause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (juegoPausado) Reanudar();
            else Pausar();
        }
    }

    public void Pausar()
    {
        Time.timeScale = 0f;
        juegoPausado = true;
        panelPausa.SetActive(true);
        controlsMenu.SetActive(false);
        FocusButton(firstButtonPause);
    }

    public void Reanudar()
    {
        UI_PlayClick();
        Time.timeScale = 1f;
        panelPausa.SetActive(false);
        controlsMenu.SetActive(false);
        juegoPausado = false;
    }

    // --- MENU NAVIGATION ---

    public void OpenOptions()
    {
        UI_PlayClick();
        panelPausa.SetActive(false);
        controlsMenu.SetActive(true);
        FocusButton(firstButtonControls);
    }

    public void BackToPause()
    {
        UI_PlayClick();
        controlsMenu.SetActive(false);
        panelPausa.SetActive(true);
        FocusButton(firstButtonPause);
    }

    public void SetMusicVolume(float sliderValue)
    {
        float dB = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20;
        mainMixer.SetFloat("music", dB);
    }

    public void Salir(string nombreEscena)
    {
        UI_PlayClick();
        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreEscena);
    }

    private void FocusButton(GameObject target)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(target);
    }
}