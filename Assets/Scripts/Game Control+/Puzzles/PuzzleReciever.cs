using UnityEngine;
using UnityEngine.InputSystem;

public class PuzzleReceiver : MonoBehaviour
{
    [Header("Puzzle Logic")]
    public string puzzleID;
    public int leversNeeded = 1;
    private int currentLeversActivated = 0;
    public bool isLocked = true;

    private bool playerInZone = false;
    private PlayerInput _playerInput;
    private Movement _playerMovement;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();

        // --- NEW LOGIC FOR 0 LEVER DOORS ---
        if (leversNeeded <= 0)
        {
            isLocked = false;
            // Ensure the animator exists before setting the parameter
            if (animator != null)
            {
                animator.SetBool("abir", true);
            }
        }
    }

    public void RegisterLeverActivation(string incomingID)
    {
        if (incomingID == puzzleID)
        {
            currentLeversActivated++;
            if (currentLeversActivated >= leversNeeded)
            {
                UnlockDoor();
            }
        }
    }

    // Extracted to a method to keep code DRY (Don't Repeat Yourself)
    private void UnlockDoor()
    {
        isLocked = false;
        Debug.Log("Door fully unlocked!");
        if (animator != null) animator.SetBool("abir", true);
    }

    private void Update()
    {
        if (playerInZone && !isLocked && _playerInput != null)
        {
            if (_playerInput.actions["Interact"].WasPressedThisFrame())
            {
                TransitionToNextRoom();
                _playerMovement._rb.linearVelocity = Vector2.zero;

            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            if (_playerInput == null) _playerInput = other.GetComponent<PlayerInput>();
            if (_playerMovement == null) _playerMovement = other.GetComponent<Movement>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
        }
    }

    void TransitionToNextRoom()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.LoadNextRoom();
        }
    }

    public bool IsUnlocked()
    {
        return !isLocked;
    }
}