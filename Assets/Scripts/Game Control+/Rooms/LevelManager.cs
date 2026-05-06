using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Reflection; // Added for resetting private variables

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

        if (_deathAnimator != null) _deathAnimator.gameObject.SetActive(false);
        if (_standardAnimator != null) _standardAnimator.gameObject.SetActive(true);
    }

    private void Start()
    {
        if (roomPrefabs.Length > 0) LoadRoom(0);
        if (_shouldFadeOutOnArrival) StartCoroutine(EntrySequence());
    }

    public void ResetOnDeath()
    {
        if (isTransitioning) return;
        StartCoroutine(DeathSequence());
    }

    public void ResetCurrentRoom()
    {
        if (isTransitioning) return;
        StartCoroutine(InternalRoomTransition());
    }

    private IEnumerator InternalRoomTransition()
    {
        isTransitioning = true;
        TogglePlayerControl(false);
        SetGlobalUIAlpha(0);

        _standardAnimator.Play(_hideScreenAnim);
        yield return new WaitForSeconds(_animDuration);

        ResetPlayerState();
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

        if (_standardAnimator != null) _standardAnimator.gameObject.SetActive(false);
        if (_deathAnimator != null) _deathAnimator.gameObject.SetActive(true);

        _deathAnimator.Play(_deathHideAnim);
        yield return new WaitForSeconds(_animDuration);

        if (_deathSfx != null && _audioSource != null)
            _audioSource.PlayOneShot(_deathSfx);

        ResetPlayerState();
        LoadRoom(currentRoomIndex);

        _deathAnimator.Play(_deathShowAnim);
        yield return new WaitForSeconds(_animDuration);

        if (_deathAnimator != null) _deathAnimator.gameObject.SetActive(false);
        if (_standardAnimator != null)
        {
            _standardAnimator.gameObject.SetActive(true);
            _standardAnimator.Play(_showScreenAnim, 0, 1f);
        }

        SetGlobalUIAlpha(1);
        TogglePlayerControl(true);
        isTransitioning = false;
    }

    private IEnumerator EntrySequence()
    {
        _shouldFadeOutOnArrival = false;
        ResetPlayerState();
        _standardAnimator.Play(_showScreenAnim);
        yield return new WaitForSeconds(_animDuration);
        TogglePlayerControl(true);
    }

    public void LoadNextRoom()
    {
        if (isTransitioning) return;
        currentRoomIndex++;
        if (currentRoomIndex < roomPrefabs.Length) StartCoroutine(InternalRoomTransition());
        else StartCoroutine(SceneTransitionSequence());
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
            if (rb != null) rb.linearVelocity = Vector2.zero;

            Transform spawnPoint = currentRoomInstance.transform.Find("EntranceSpawnPoint");
            if (spawnPoint != null) player.transform.position = spawnPoint.position;
        }
    }

    private void ResetPlayerState()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        // 1. Core Movement Reset
        Movement move = player.GetComponent<Movement>();
        if (move != null)
        {
            move.usingFireMagic = false;
            move.usingWindMagic = false;
            move.usingWaterMagic = false;
            move._rb.linearVelocity = Vector2.zero;

            // Reset Animator Booleans (Assuming your animator uses these names)
            Animator anim = player.GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetBool("Fire", false);
                anim.SetBool("Wind", false);
                anim.SetBool("Water", false);
                anim.Play("Idle"); // Force back to Idle state
            }
        }

        // 2. Reset private _isFireMagicToggled in FireMagic script via Reflection
        FireMagic fire = player.GetComponent<FireMagic>();
        if (fire != null)
        {
            var field = typeof(FireMagic).GetField("_isFireMagicToggled", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null) field.SetValue(fire, false);
        }

        // 3. Reset WaterMagic
        WaterMagic water = player.GetComponent<WaterMagic>();
        if (water != null) water.DashUsed = false;

        // 4. Force Stats back to default
        WindMagic wind = player.GetComponent<WindMagic>();
        if (wind != null)
        {
            // Reset scriptable stats via your StopGliding values
            var statsField = typeof(Movement).GetField("_stats", BindingFlags.NonPublic | BindingFlags.Instance);
            ScriptableStats stats = statsField?.GetValue(move) as ScriptableStats;
            if (stats != null)
            {
                stats.MaxFallSpeed = 40;
                stats.MaxSpeed = 14;
            }
        }
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