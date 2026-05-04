using UnityEngine;
using UnityEngine.InputSystem;

/* * HOW TO USE:
 * 1. Attach to a "Hiding Spot" prefab (e.g., a vent or closet).
 * 2. Assign the 'Hidespot' Rigidbody2D (center of the object).
 * 3. Assign the 'Empty' and 'Occupied' sprites.
 * 4. Assign the 'Interact Sound' clip in the Inspector.
 */

public class HideyScript : MonoBehaviour
{
    [Header("Player References")]
    [SerializeField] private Movement _move;
    [SerializeField] private PlayerInput _input;
    public Rigidbody2D agnes;

    [Header("Hiding Spot Settings")]
    public Rigidbody2D hidespot;
    [SerializeField] private Sprite emptySprite;
    [SerializeField] private Sprite occupiedSprite;

    [Header("Audio")]
    [SerializeField] private AudioClip interactSound; // The sound of entering/exiting the vent
    private AudioSource audioSource;

    private bool playerInRange = false;
    private CapsuleCollider2D disableCollision;
    private SpriteRenderer playerSprite;
    private SpriteRenderer spotSpriteRenderer;

    private void Start()
    {
        spotSpriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component

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

        if (spotSpriteRenderer != null && emptySprite != null)
        {
            spotSpriteRenderer.sprite = emptySprite;
        }
    }

    private void Update()
    {
        if (_input == null || _move == null) return;

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

        // Update Appearance
        if (spotSpriteRenderer != null && occupiedSprite != null)
            spotSpriteRenderer.sprite = occupiedSprite;

        PlayInteractionSound();
    }

    private void Unhide()
    {
        _move.isHiding = false;
        if (disableCollision != null) disableCollision.isTrigger = false;
        if (playerSprite != null) playerSprite.enabled = true;
        agnes.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Update Appearance
        if (spotSpriteRenderer != null && emptySprite != null)
            spotSpriteRenderer.sprite = emptySprite;
    }

    private void PlayInteractionSound()
    {
        // Play the sound if both the source and clip exist
        if (audioSource != null && interactSound != null)
        {
            audioSource.PlayOneShot(interactSound);
        }
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