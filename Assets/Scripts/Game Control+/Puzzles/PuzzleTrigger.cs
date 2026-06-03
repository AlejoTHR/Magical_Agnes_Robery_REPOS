using UnityEngine;
using UnityEngine.InputSystem;

public class PuzzleTrigger : MonoBehaviour
{
    [Header("Connection IDs")]
    public string puzzleID;
    public string specificLeverID;

    private bool isPulled = false;
    private bool playerInZone = false;
    private PlayerInput _playerInput;

    private Animator animator;

    [Header("Audio")]
    [SerializeField] AudioClip _leverSFX;
    [SerializeField] AudioSource _leverSFXSource;

    private void Start()
    {
        animator = GetComponent<Animator>();
        _leverSFXSource.clip = _leverSFX;
    }

    private void Update()
    {
        if (playerInZone && !isPulled && _playerInput != null)
        {
            if (_playerInput.actions["Interact"].WasPressedThisFrame())
            {
                ExecuteTrigger();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            if (_playerInput == null) _playerInput = other.GetComponent<PlayerInput>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
        }
    }

    private void ExecuteTrigger()
    {
        isPulled = true;
        SendSignals();
        animator.SetBool("IsPulled", true);
        _leverSFXSource.Play();

        // Visual feedback
        if (TryGetComponent<SpriteRenderer>(out var sr))
        {
            sr.color = Color.gray;
        }
    }

    void SendSignals()
    {
        // Find and notify receivers
        PuzzleReceiver[] receivers = Object.FindObjectsByType<PuzzleReceiver>(FindObjectsSortMode.None);
        foreach (var receiver in receivers)
        {
            receiver.RegisterLeverActivation(puzzleID);
        }

        // Find and notify lights
        PuzzleLightCue[] lights = Object.FindObjectsByType<PuzzleLightCue>(FindObjectsSortMode.None);
        foreach (var light in lights)
        {
            light.ActivateLight(specificLeverID);
        }
    }

    public bool IsActivated()
    {
        return isPulled;
    }
}