using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [Header("Master Audio Clips")]
    [SerializeField] private AudioClip _walkClip;
    [SerializeField] private AudioClip _windGlideClip;
    [SerializeField] private AudioClip _fireCannonballClip;
    [SerializeField] private AudioClip _waterDashClip;
    [SerializeField][Range(0, 1)] private float _masterVolume = 0.5f;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        
        _audioSource.playOnAwake = false;
        _audioSource.volume = _masterVolume;
    }
    void Start()
    {
        _audioSource.playOnAwake = false;
        _audioSource.volume = _masterVolume;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
