using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;

public class StartMenuManager : MonoBehaviour
{
    [Header("UI Setup")]
    public GameObject singularButton;
    public string sceneToLoad;

    [Header("Transition Settings")]
    [SerializeField] private Animator _standardAnimator;
    [SerializeField] private string _showScreenAnim = "FadeOut"; // Clears the screen on start
    [SerializeField] private string _hideScreenAnim = "FadeIn";  // Darkens the screen on click
    [SerializeField] private float _animDuration = 1.0f;

    private bool isTransitioning = false;

    void Start()
    {
        // 1. Controller Focus
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(singularButton);
        }

        // 2. Play the Fade Out (Arrival) immediately
        if (_standardAnimator != null)
        {
            _standardAnimator.Play(_showScreenAnim);
        }
    }

    // Link this to your Button's OnClick event
    public void OnButtonPressed()
    {
        if (isTransitioning) return;
        StartCoroutine(ExitSequence());
    }

    private IEnumerator ExitSequence()
    {
        isTransitioning = true;

        // 3. Play the Fade In (Departure)
        if (_standardAnimator != null)
        {
            _standardAnimator.Play(_hideScreenAnim);
        }

        yield return new WaitForSeconds(_animDuration);

        // 4. Load the next scene
        SceneManager.LoadScene(sceneToLoad);
    }
}