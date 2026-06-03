using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;

public class StartMenuManager : MonoBehaviour
{
    public static StartMenuManager Instance;

    [Header("UI Setup")]
    public GameObject singularButton;
    public string sceneToLoad;

    [Header("Sound Settings")]
    public AudioClip _globalClickSound;
    private AudioSource _hoverChannel;
    private AudioSource _clickChannel;

    [Header("Transition Settings")]
    [SerializeField] private Animator _standardAnimator;
    [SerializeField] private string _showScreenAnim = "FadeOut"; 
    [SerializeField] private string _hideScreenAnim = "FadeIn"; 
    [SerializeField] private float _animDuration = 1.0f;

    private bool isTransitioning = false;

    void Awake()
    {
        Instance = this;
        _hoverChannel = gameObject.AddComponent<AudioSource>();
        _clickChannel = gameObject.AddComponent<AudioSource>();

        _clickChannel.priority = 0;
        _hoverChannel.playOnAwake = false;
        _clickChannel.playOnAwake = false;
    }

    void Start()
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(singularButton);
        }


        if (_standardAnimator != null)
        {
            _standardAnimator.Play(_showScreenAnim);
        }
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

        CancelInvoke(nameof(StopClickAudio));
        Invoke(nameof(StopClickAudio), 0.4f);
    }

    private void StopClickAudio() => _clickChannel.Stop();

    public void OnButtonPressed()
    {
        if (isTransitioning) return;
        UI_PlayClick();
        StartCoroutine(ExitSequence());
    }

    private IEnumerator ExitSequence()
    {
        isTransitioning = true;

        if (_standardAnimator != null)
        {
            _standardAnimator.Play(_hideScreenAnim);
        }

        yield return new WaitForSeconds(_animDuration);

        SceneManager.LoadScene(sceneToLoad);
    }
}