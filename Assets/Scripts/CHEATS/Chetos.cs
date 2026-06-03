using UnityEngine;
using UnityEngine.SceneManagement; 

public class MagicManager : MonoBehaviour
{
    [Header("Magic Script References")]
    [SerializeField] private FireMagic fireMagic;
    [SerializeField] private WaterMagic waterMagic;
    [SerializeField] private WindMagic windMagic;

    private void Update()
    {
        HandleMagicInputs();
        HandleCheatInputs();
    }

    private void HandleMagicInputs()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && fireMagic != null) fireMagic.enabled = !fireMagic.enabled;
        if (Input.GetKeyDown(KeyCode.Alpha2) && waterMagic != null) waterMagic.enabled = !waterMagic.enabled;
        if (Input.GetKeyDown(KeyCode.Alpha3) && windMagic != null) windMagic.enabled = !windMagic.enabled;

        if (Input.GetKeyDown(KeyCode.Alpha5)) SetAllMagicState(false);
        if (Input.GetKeyDown(KeyCode.Alpha6)) SetAllMagicState(true);
    }

    private void HandleCheatInputs()
    {
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                Debug.Log("Cheat: Skipping to next Scene...");
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                Debug.LogWarning("No more scenes in Build Settings!");
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha9) && LevelManager.Instance != null)
        {
            LevelManager.Instance.LoadNextRoom();
        }

        if (Input.GetKeyDown(KeyCode.Alpha0)) UnlockEverything();
    }

    private void SetAllMagicState(bool state)
    {
        if (fireMagic) fireMagic.enabled = state;
        if (waterMagic) waterMagic.enabled = state;
        if (windMagic) windMagic.enabled = state;
    }

    private void UnlockEverything()
    {
        PuzzleReceiver[] receivers = Object.FindObjectsByType<PuzzleReceiver>(FindObjectsSortMode.None);
        foreach (var r in receivers) r.isLocked = false;
        Debug.Log("Cheats: All room puzzles cleared.");
    }
}