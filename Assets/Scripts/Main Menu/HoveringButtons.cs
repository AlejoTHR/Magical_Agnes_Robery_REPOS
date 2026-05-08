using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonAudio : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler
{
    [Header("Audio")]
    public AudioClip hoverSound;

    [Header("Scaling Animation")]
    public float scaleMultiplier = 1.15f;
    public float animationSpeed = 15f;

    [Header("Idle Floating (Unfocused Only)")]
    public float bobAmount = 8f;
    public float bobSpeed = 3f;
    private float randomOffset; // To prevent all buttons from bobbing in perfect unison

    private Vector3 initialScale;
    private Vector2 initialPosition;
    private RectTransform rectTransform;
    private bool isSelected = false;
    private TMP_Wave waveScript;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        initialScale = transform.localScale;
        initialPosition = rectTransform.anchoredPosition;
        waveScript = GetComponentInChildren<TMP_Wave>();
        randomOffset = Random.Range(0f, 5f); // Diversifies the wave start
    }

    void Update()
    {
        // 1. Scaling Logic
        Vector3 targetScale = isSelected ? initialScale * scaleMultiplier : initialScale;
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * animationSpeed);

        // 2. Swapped Bobbing Logic
        if (!isSelected)
        {
            // Hover/Bob while NOT selected
            float newY = initialPosition.y + (Mathf.Sin((Time.unscaledTime + randomOffset) * bobSpeed) * bobAmount);
            rectTransform.anchoredPosition = new Vector2(initialPosition.x, newY);
        }
        else
        {
            // Stay still at the initial position while focused
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, initialPosition, Time.unscaledDeltaTime * animationSpeed);
        }
    }

    public void OnPointerEnter(PointerEventData eventData) => EventSystem.current.SetSelectedGameObject(gameObject);

    public void OnPointerExit(PointerEventData eventData)
    {
        if (EventSystem.current.currentSelectedGameObject == gameObject)
            EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (!isSelected)
        {
            isSelected = true;
            if (MainMenu.Instance != null) MainMenu.Instance.PlayHoverSound(hoverSound);
            else if (MenuPausa.Instance != null) MenuPausa.Instance.PlayHoverSound(hoverSound);
            if (waveScript != null) waveScript.isHovered = true;
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        isSelected = false;
        if (waveScript != null) waveScript.ResetText();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (MainMenu.Instance != null) MainMenu.Instance.UI_PlayClick();
        else if (MenuPausa.Instance != null) MenuPausa.Instance.UI_PlayClick();
    }
}