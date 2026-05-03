using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public static bool _shouldFadeOutOnArrival = false;

    [Header("Room List")]
    public GameObject[] roomPrefabs;

    [Header("Standard Animator")]
    [SerializeField] private Animator _standardAnimator;
    [SerializeField] private string _hideScreenAnim = "FadeIn";
    [SerializeField] private string _showScreenAnim = "FadeOut";

    [Header("Death Animator")]
    [SerializeField] private Animator _deathAnimator;
    [SerializeField] private string _deathHideAnim = "DeathEntry";
    [SerializeField] private string _deathShowAnim = "DeathExit";

    [Header("Settings")]
    [SerializeField] private float _animDuration = 1.0f;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _deathSfx;

    private GameObject currentRoomInstance;
    private int currentRoomIndex = 0;
    private bool isTransitioning = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (_audioSource == null) _audioSource = GetComponent<AudioSource>();

        // Initialization: Standard ON, Death OFF
        if (_deathAnimator != null) _deathAnimator.gameObject.SetActive(false);
        if (_standardAnimator != null) _standardAnimator.gameObject.SetActive(true);
    }

    private void Start()
    {
        if (roomPrefabs.Length > 0) LoadRoom(0);

        if (_shouldFadeOutOnArrival)
        {
            StartCoroutine(EntrySequence());
        }
    }

    // --- PUBLIC TRIGGERS ---

    public void ResetOnDeath()
    {
        if (isTransitioning) return;
        Debug.Log("Death Sequence Started");
        StartCoroutine(DeathSequence());
    }

    public void ResetCurrentRoom()
    {
        if (isTransitioning) return;
        Debug.Log("Standard Room Reset Started");
        StartCoroutine(InternalRoomTransition());
    }

    // --- TRANSITION LOGIC ---

    private IEnumerator InternalRoomTransition()
    {
        isTransitioning = true;
        TogglePlayerControl(false);
        SetGlobalUIAlpha(0);

        // Ensure we are using the Standard Animator
        _deathAnimator.gameObject.SetActive(false);
        _standardAnimator.gameObject.SetActive(true);

        _standardAnimator.Play(_hideScreenAnim);
        yield return new WaitForSeconds(_animDuration);

        LoadRoom(currentRoomIndex);

        _standardAnimator.Play(_showScreenAnim);
        yield return new WaitForSeconds(_animDuration);

        SetGlobalUIAlpha(1);
        TogglePlayerControl(true);
        isTransitioning = false;
    }

    private IEnumerator DeathSequence()
    {
        isTransitioning = true;
        TogglePlayerControl(false);
        SetGlobalUIAlpha(0);

        // Swap Animators
        if (_standardAnimator != null) _standardAnimator.gameObject.SetActive(false);
        if (_deathAnimator != null) _deathAnimator.gameObject.SetActive(true);

        // 1. Play Death Entry
        _deathAnimator.Play(_deathHideAnim);

        // Timing logic for the gunshot
        float soundLeadTime = 0.25f;
        float firstWait = Mathf.Max(0, _animDuration - soundLeadTime);

        yield return new WaitForSeconds(firstWait);

        if (_deathSfx != null && _audioSource != null)
            _audioSource.PlayOneShot(_deathSfx);

        yield return new WaitForSeconds(soundLeadTime);

        // 2. Perform the actual room reload
        LoadRoom(currentRoomIndex);

        // 3. Play Death Exit (Screen clears up)
        _deathAnimator.Play(_deathShowAnim);
        yield return new WaitForSeconds(_animDuration);

        // --- CRITICAL CLEANUP ---
        // Ensure we switch back to standard animator so regular transitions work again
        if (_deathAnimator != null) _deathAnimator.gameObject.SetActive(false);
        if (_standardAnimator != null)
        {
            _standardAnimator.gameObject.SetActive(true);
            // Force the standard animator back to its "Idle" or "Visible" state
            _standardAnimator.Play(_showScreenAnim, 0, 1f);
        }

        SetGlobalUIAlpha(1);
        TogglePlayerControl(true);

        // Release the lock so other transitions can fire
        isTransitioning = false;
    }

    private IEnumerator EntrySequence()
    {
        _shouldFadeOutOnArrival = false;
        _standardAnimator.gameObject.SetActive(true);
        _standardAnimator.Play(_showScreenAnim);
        yield return new WaitForSeconds(_animDuration);
        TogglePlayerControl(true);
    }

    // --- HELPER METHODS ---

    public void LoadNextRoom()
    {
        if (isTransitioning) return;
        currentRoomIndex++;
        if (currentRoomIndex < roomPrefabs.Length)
            StartCoroutine(InternalRoomTransition());
        else
            StartCoroutine(SceneTransitionSequence());
    }

    private IEnumerator SceneTransitionSequence()
    {
        isTransitioning = true;
        TogglePlayerControl(false);
        _standardAnimator.Play(_hideScreenAnim);
        yield return new WaitForSeconds(_animDuration);
        _shouldFadeOutOnArrival = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void LoadRoom(int index)
    {
        if (currentRoomInstance != null) Destroy(currentRoomInstance);

        currentRoomInstance = Instantiate(roomPrefabs[index], Vector3.zero, Quaternion.identity);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero; // Fixed: Use .velocity in modern Unity

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
        GameObject[] interactionUIs = GameObject.FindGameObjectsWithTag("InteractionUI");
        foreach (GameObject ui in interactionUIs)
        {
            CanvasGroup group = ui.GetComponent<CanvasGroup>();
            if (group != null)
            {
                group.alpha = alpha;
                group.blocksRaycasts = (alpha > 0);
            }
        }
    }
}