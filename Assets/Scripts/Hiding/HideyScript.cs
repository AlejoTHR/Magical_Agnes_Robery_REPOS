using UnityEngine;
using UnityEngine.InputSystem;

/* * HOW TO USE:
 * 1. Attach to a "Hiding Spot" prefab (e.g., a closet or bush).
 * 2. Assign the 'Hidespot' Rigidbody2D (center of the object).
 * 3. When the player interacts, they are snapped to this position and 'isHiding' is set to true.
 * 4. While hiding, enemies using 'EnemyScript' will not detect the player.
 */

public class HideyScript : MonoBehaviour
{
    [SerializeField] private Movement _move;
    [SerializeField] private PlayerInput _input; // Drag Player object here
    public Rigidbody2D agnes;
    public Rigidbody2D hidespot;

    private bool playerInRange = false;
    private CapsuleCollider2D disableCollision;
    private SpriteRenderer playerSprite;

    private void Start()
    {
        if (agnes == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) agnes = playerObj.GetComponent<Rigidbody2D>();
        }

        if (agnes != null)
        {
            disableCollision = agnes.GetComponent<CapsuleCollider2D>();
            _move = agnes.GetComponent<Movement>();
            playerSprite = agnes.GetComponent<SpriteRenderer>(); 
            if (_input == null) _input = agnes.GetComponent<PlayerInput>();
        }
    }

    private void Update()
    {
        if (_input == null || _move == null) return;

        // Use the "Interact" action mapped to A (Pro Controller) or E/H (Keyboard)
        if (playerInRange && _input.actions["Interact"].WasPressedThisFrame())
        {
            if (!_move.isHiding) Hide();
            else Unhide();
        }

        if (_move.isHiding && playerInRange)
        {
            agnes.transform.position = hidespot.transform.position;
            agnes.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
        }
    }

    private void Hide()
    {
        _move.isHiding = true;
        if (disableCollision != null) disableCollision.isTrigger = true;
        if (playerSprite != null) playerSprite.enabled = false; 
    }

    private void Unhide()
    {
        _move.isHiding = false;
        if (disableCollision != null) disableCollision.isTrigger = false;
        if (playerSprite != null) playerSprite.enabled = true; 
        agnes.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (_move.isHiding) Unhide();
            playerInRange = false;
        }
    }
}