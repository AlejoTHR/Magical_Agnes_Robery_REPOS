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

        _standardAnimator.gameObject.SetActive(false);
        _deathAnimator.gameObject.SetActive(true);

        // 1. Start the animation
        _deathAnimator.Play(_deathHideAnim);

        // 2. Define your "Early Trigger" offset
        float soundLeadTime = 0.2f; // Trigger sound 0.2s before the end
        float firstWait = Mathf.Max(0, _animDuration - soundLeadTime);

        // 3. Wait for the majority of the animation
        yield return new WaitForSeconds(firstWait);

        // 4. Fire the gunshot slightly early
        if (_deathSfx != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(_deathSfx);
        }

        // 5. Wait for the remaining fraction of the animation
        yield return new WaitForSeconds(soundLeadTime);

        // 6. Proceed to load the room once screen is fully obscured
        LoadRoom(currentRoomIndex);

        _deathAnimator.Play(_deathShowAnim);
        yield return new WaitForSeconds(_animDuration);

        _deathAnimator.gameObject.SetActive(false);
        _standardAnimator.gameObject.SetActive(true);

        SetGlobalUIAlpha(1);
        TogglePlayerControl(true);
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