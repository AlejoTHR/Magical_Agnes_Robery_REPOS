using UnityEngine;
using UnityEngine.InputSystem;

/* * HOW TO USE:
 * 1. Attach to a Lever or Pressure Plate.
 * 2. Set 'Puzzle ID' to the door it belongs to.
 * 3. Set 'Specific Lever ID' to the specific light cue it should activate.
 * 4. Requires a Collider2D (Trigger) for the player to stand in.
 */
public class PuzzleTrigger : MonoBehaviour
{
    [Header("Connection IDs")]
    public string puzzleID;
    public string specificLeverID;

    private bool isPulled = false;
    private bool playerInZone = false;
    private PlayerInput _playerInput;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // We check input in Update for maximum responsiveness
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
            // Cache the reference once when they enter
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