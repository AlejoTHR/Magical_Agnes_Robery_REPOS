using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MenuPausa : MonoBehaviour
{
    public GameObject panelPausa;
    public GameObject controlsMenu;
    public AudioMixer mainMixer;
    public Slider musicSlider;

    [Header("Controller Setup")]
    public GameObject firstButtonPause;    // Assign the 'Resume' button here
    public GameObject firstButtonControls; // Assign the 'Music Slider' or 'Back' button here

    public bool juegoPausado;

    void Start()
    {
        float currentVol;
        // Using "music" to match your Mixer's exposed parameter
        if (mainMixer.GetFloat("music", out currentVol))
        {
            musicSlider.value = Mathf.Pow(10, currentVol / 20);
        }
    }

    public void SetMusicVolume(float sliderValue)
    {
        // Converts linear slider 0.0001-1 into logarithmic -80dB to 0dB
        float dB = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20;
        mainMixer.SetFloat("music", dB);
    }

    // --- NEW FUNCTIONS FOR NAVIGATION ---

    public void OpenOptions()
    {
        panelPausa.SetActive(false);
        controlsMenu.SetActive(true);

        // Tell the controller to focus on the first item in the Options menu
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstButtonControls);
    }

    public void BackToPause()
    {
        controlsMenu.SetActive(false);
        panelPausa.SetActive(true);

        // Tell the controller to focus back on the main pause buttons
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstButtonPause);
    }

    // --- EXISTING LOGIC ---

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

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstButtonPause);
    }

    public void Reanudar()
    {
        Time.timeScale = 1f;
        panelPausa.SetActive(false);
        controlsMenu.SetActive(false);
        juegoPausado = false;
    }

    public void Salir(string nombreEscena)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreEscena);
    }
}