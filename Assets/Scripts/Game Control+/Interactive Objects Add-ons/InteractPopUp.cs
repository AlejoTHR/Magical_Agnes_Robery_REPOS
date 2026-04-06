using UnityEngine;

public class InteractPopup : MonoBehaviour
{
    [Header("Popup")]
    public GameObject popupUI;

    private PuzzleTrigger puzzleTrigger;
    private PuzzleReceiver puzzleReceiver;

    private void Start()
    {
        if (popupUI != null)
            popupUI.SetActive(false);

        // Detectamos si este objeto tiene alguno de los dos scripts
        puzzleTrigger = GetComponent<PuzzleTrigger>();
        puzzleReceiver = GetComponent<PuzzleReceiver>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Caso palanca: ya activada  no mostrar popup
            if (puzzleTrigger != null && puzzleTrigger.IsActivated())
                return;

            // Caso puerta: bloqueada  no mostrar popup
            if (puzzleReceiver != null && !puzzleReceiver.IsUnlocked())
                return;

            if (popupUI != null)
                popupUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (popupUI != null)
                popupUI.SetActive(false);
        }
    }
}