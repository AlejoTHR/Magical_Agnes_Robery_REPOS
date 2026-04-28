using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    // Static handshake to tell the NEXT scene to play the "Open" animation
    public static bool _shouldFadeOutOnArrival = false;

    [Header("Room List")]
    public GameObject[] roomPrefabs;

    [Header("Transitions")]
    [SerializeField] private Animator _transitionAnimator;
    [SerializeField] private string _hideScreenAnim = "FadeIn";  // Close curtain
    [SerializeField] private string _showScreenAnim = "FadeOut"; // Open curtain
    [SerializeField] private float _animDuration = 1.0f;

    private GameObject currentRoomInstance;
    private int currentRoomIndex = 0;
    private bool isTransitioning = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // 1. Setup initial room
        if (roomPrefabs.Length > 0)
        {
            LoadRoom(0);
        }

        // 2. ONLY play the "Open" animation if we just arrived or reset
        // If it's the very first time opening the game, you might want this true by default
        if (_shouldFadeOutOnArrival && _transitionAnimator != null)
        {
            StartCoroutine(EntrySequence());
        }
    }

    public void LoadNextRoom()
    {
        if (isTransitioning) return;

        currentRoomIndex++;

        if (currentRoomIndex < roomPrefabs.Length)
        {
            // Room swap in same scene
            StartCoroutine(InternalRoomTransition());
        }
        else
        {
            // Moving to next scene
            StartCoroutine(SceneTransitionSequence());
        }
    }

    private IEnumerator EntrySequence()
    {
        _shouldFadeOutOnArrival = false; // Reset flag
        _transitionAnimator.Play(_showScreenAnim);
        yield return new WaitForSeconds(_animDuration);
        TogglePlayerControl(true);
    }

    // Sequence for internal room swap: Fade In -> Swap -> Fade Out
    private IEnumerator InternalRoomTransition()
    {
        isTransitioning = true;
        TogglePlayerControl(false);

        // 1. MUTE THE UI
        SetGlobalUIAlpha(0);

        _transitionAnimator.Play(_hideScreenAnim);
        yield return new WaitForSeconds(_animDuration);

        LoadRoom(currentRoomIndex);

        _transitionAnimator.Play(_showScreenAnim);
        yield return new WaitForSeconds(_animDuration);

        // 2. RESTORE THE UI
        SetGlobalUIAlpha(1);

        TogglePlayerControl(true);
        isTransitioning = false;
    }

    // Sequence for scene jump: Fade In -> Load Scene
    private IEnumerator SceneTransitionSequence()
    {
        isTransitioning = true;
        TogglePlayerControl(false);

        // Close Curtain
        _transitionAnimator.Play(_hideScreenAnim);
        yield return new WaitForSeconds(_animDuration);

        _shouldFadeOutOnArrival = true; // Tell next scene to "Open"
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ResetCurrentRoom()
    {
        if (isTransitioning) return;
        StartCoroutine(InternalRoomTransition()); // Reuse the Close -> Swap -> Open logic
    }

    private void LoadRoom(int index)
    {
        if (currentRoomInstance != null) Destroy(currentRoomInstance);
        currentRoomInstance = Instantiate(roomPrefabs[index], Vector3.zero, Quaternion.identity);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;

            Transform spawnPoint = currentRoomInstance.transform.Find("EntranceSpawnPoint");
            if (spawnPoint != null) player.transform.position = spawnPoint.position;
        }

        RoomController rc = currentRoomInstance.GetComponent<RoomController>();
        if (rc != null) rc.ActivateRoom();
    }

    private void TogglePlayerControl(bool state)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Movement moveScript = player.GetComponent<Movement>();
            if (moveScript != null) moveScript.enabled = state;
        }
    }

    private void SetGlobalUIAlpha(float alpha)
    {
        // Find all objects with the tag you assigned to your Interaction UI
        GameObject[] interactionUIs = GameObject.FindGameObjectsWithTag("InteractionUI");

        foreach (GameObject ui in interactionUIs)
        {
            CanvasGroup group = ui.GetComponent<CanvasGroup>();
            if (group != null)
            {
                group.alpha = alpha;
                // Optional: Block interaction while invisible
                group.blocksRaycasts = (alpha > 0);
            }
        }
    }
}