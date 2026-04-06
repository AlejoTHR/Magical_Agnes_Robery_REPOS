using UnityEngine;
using UnityEngine.InputSystem;

/* * HOW TO USE:
 * 1. Attach to a Door or Level Exit.
 * 2. Set 'Puzzle ID' to match the levers required to open it.
 * 3. Set 'Levers Needed' (e.g., 3 levers to unlock).
 * 4. The door remains 'Locked' until all signals are received; then the player can press 'Interact' to exit.
 */

public class PuzzleReceiver : MonoBehaviour
{
    [Header("Puzzle Logic")]
    public string puzzleID;
    public int leversNeeded = 1;
    private int currentLeversActivated = 0;
    public bool isLocked = true;

    private bool playerInZone = false;
    private PlayerInput _playerInput;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void RegisterLeverActivation(string incomingID)
    {
        if (incomingID == puzzleID)
        {
            currentLeversActivated++;
            if (currentLeversActivated >= leversNeeded)
            {
                isLocked = false;
                Debug.Log("Door fully unlocked!");
                animator.SetBool("abir", true);
            }
        }
    }

    private void Update()
    {
        // Check for the "Interact" press every frame if the player is standing at the door
        if (playerInZone && !isLocked && _playerInput != null)
        {
            if (_playerInput.actions["Interact"].WasPressedThisFrame())
            {
                TransitionToNextRoom();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            // Cache reference once
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