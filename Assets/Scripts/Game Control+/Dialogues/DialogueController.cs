/* * =================================================================================================
 * DIALOGUE CONTROLLER: THE ULTIMATE DDLC-STYLE SYSTEM (V5.1 - VISIBILITY FIX)
 * =================================================================================================
 * * --- THE FIX ---
 * Added logic to explicitly SetActive(false) on the Background Image and Portrait 
 * whenever 'isSignMode' is checked, ensuring signs remain "clean."
 * =================================================================================================
 */

using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[System.Serializable]
public class DialogueLine
{
    public string characterName;
    [TextArea(3, 10)]
    public string text;
    public Sprite characterPortrait;
    public Sprite backgroundOverride;
    public bool triggersLevelTransition;
    public float lineSpeed = 0.08f;
}

public class DialogueController : MonoBehaviour
{
    public enum TransitionType { LoadNextRoomPrefab, LoadSpecificScene }

    [Header("Mode & Trigger Settings")]
    public bool isSignMode = false;
    public bool playOnEnter = false;

    [Header("Input Locking")]
    public MonoBehaviour playerMovementScript;
    public string pauseButtonInput = "Cancel";

    [Header("Transition Settings")]
    public TransitionType transitionType;
    public string sceneToLoad;
    public Image fadeOverlay;
    public float fadeDuration = 1.0f;

    [Header("UI References")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public Image portraitImage;
    public Image backgroundImage;

    [Header("Sequence")]
    public List<DialogueLine> dialogueSequence = new List<DialogueLine>();

    private int index = 0;
    private bool isPlayerInRange;
    private bool isTyping;
    private bool isDialogueActive = false;
    private Coroutine currentTypewriter;

    void Start()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);

        if (playerMovementScript == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerMovementScript = player.GetComponent<MonoBehaviour>();
        }

        if (fadeOverlay != null)
            fadeOverlay.color = new Color(fadeOverlay.color.r, fadeOverlay.color.g, fadeOverlay.color.b, 0);
    }

    void Update()
    {
        if (isDialogueActive)
        {
            if (!string.IsNullOrEmpty(pauseButtonInput) && Input.GetButtonDown(pauseButtonInput)) return;

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (isTyping) FinishLineInstantly();
                else AdvanceOrEnd();
            }
            return;
        }

        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && !playOnEnter)
        {
            StartDialogueSequence();
        }
    }

    private void StartDialogueSequence()
    {
        StopAllCoroutines();
        index = 0;
        isDialogueActive = true;
        dialoguePanel.SetActive(true);

        if (!isSignMode) TogglePlayerControls(false);

        StartCoroutine(DisplayLine());
    }

    void AdvanceOrEnd()
    {
        if (index < dialogueSequence.Count - 1)
        {
            index++;
            StartCoroutine(DisplayLine());
        }
        else
        {
            StartCoroutine(EndDialogueSequence());
        }
    }

    IEnumerator DisplayLine()
    {
        isTyping = true;
        DialogueLine currentLine = dialogueSequence[index];

        // --- SIGN MODE VISIBILITY LOGIC ---
        if (isSignMode)
        {
            // Hide everything except the text box
            if (portraitImage != null) portraitImage.gameObject.SetActive(false);
            if (backgroundImage != null) backgroundImage.gameObject.SetActive(false);
        }
        else
        {
            // Show Portraits if they exist
            if (portraitImage != null)
            {
                bool hasPortrait = currentLine.characterPortrait != null;
                portraitImage.gameObject.SetActive(hasPortrait);
                if (hasPortrait) portraitImage.sprite = currentLine.characterPortrait;
            }

            // Show Background if a swap is requested
            if (backgroundImage != null)
            {
                if (currentLine.backgroundOverride != null)
                {
                    backgroundImage.gameObject.SetActive(true);
                    backgroundImage.sprite = currentLine.backgroundOverride;
                }
                // Note: We don't hide the background here if it's null 
                // because we want the previous background to stay visible
            }
        }

        // Name Text handling
        if (nameText != null)
        {
            bool hasName = !string.IsNullOrEmpty(currentLine.characterName);
            nameText.gameObject.SetActive(hasName);
            if (hasName) nameText.text = currentLine.characterName;
        }

        dialogueText.text = "";

        if (currentLine.lineSpeed <= 0)
        {
            dialogueText.text = currentLine.text;
        }
        else
        {
            currentTypewriter = StartCoroutine(TypeEffect(currentLine));
            yield return currentTypewriter;
        }

        isTyping = false;
    }

    IEnumerator TypeEffect(DialogueLine line)
    {
        foreach (char letter in line.text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(line.lineSpeed);
        }
        currentTypewriter = null;
    }

    void FinishLineInstantly()
    {
        if (currentTypewriter != null) StopCoroutine(currentTypewriter);
        dialogueText.text = dialogueSequence[index].text;
        isTyping = false;
    }

    IEnumerator EndDialogueSequence()
    {
        bool shouldTransition = dialogueSequence[index].triggersLevelTransition;

        isDialogueActive = false;
        dialoguePanel.SetActive(false);

        // Ensure UI is cleaned up for the next interaction
        if (backgroundImage != null) backgroundImage.gameObject.SetActive(false);
        if (portraitImage != null) portraitImage.gameObject.SetActive(false);

        if (!shouldTransition)
        {
            TogglePlayerControls(true);
        }
        else
        {
            if (fadeOverlay != null) yield return StartCoroutine(Fade(1));

            if (transitionType == TransitionType.LoadNextRoomPrefab && LevelManager.Instance != null)
                LevelManager.Instance.LoadNextRoom();
            else if (transitionType == TransitionType.LoadSpecificScene)
                SceneManager.LoadScene(sceneToLoad);

            TogglePlayerControls(true);
        }
    }

    void TogglePlayerControls(bool state)
    {
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = state;
        }
    }

    IEnumerator Fade(float targetAlpha)
    {
        if (fadeOverlay == null) yield break;
        float startAlpha = fadeOverlay.color.a;
        float timer = 0;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, timer / fadeDuration);
            fadeOverlay.color = new Color(fadeOverlay.color.r, fadeOverlay.color.g, fadeOverlay.color.b, newAlpha);
            yield return null;
        }
    }

    private void OnDisable() { TogglePlayerControls(true); }
    private void OnDestroy() { TogglePlayerControls(true); }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (playOnEnter && !isDialogueActive) StartDialogueSequence();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (isSignMode && isDialogueActive)
            {
                isDialogueActive = false;
                dialoguePanel.SetActive(false);
                // Ensure UI elements are hidden when walking away
                if (backgroundImage != null) backgroundImage.gameObject.SetActive(false);
                TogglePlayerControls(true);
            }
        }
    }
}