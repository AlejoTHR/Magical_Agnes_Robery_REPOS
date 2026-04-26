/* * HOW TO USE:
 * 1. Create one 'LevelManager' GameObject in your scene.
 * 2. Drag your Room Prefabs into the 'Room Prefabs' array in order.
 * 3. IMPORTANT: Every Room Prefab must have a child named "EntranceSpawnPoint".
 * 4. Handles room transitions and player teleportation automatically.
 */

using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Room List")]
    [Tooltip("The order of room prefabs for THIS specific scene.")]
    public GameObject[] roomPrefabs;

    private GameObject currentRoomInstance;
    private int currentRoomIndex = 0;

    private void Awake()
    {
        // Simple instance reference for the current scene
        Instance = this;
    }

    private void Start()
    {
        if (roomPrefabs.Length > 0)
        {
            LoadRoom(0);
        }
        else
        {
            Debug.LogError("No room prefabs assigned to LevelManager in this scene!");
        }
    }

    public void LoadNextRoom()
    {
        currentRoomIndex++;
        if (currentRoomIndex < roomPrefabs.Length)
        {
            LoadRoom(currentRoomIndex);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public void ResetCurrentRoom()
    {
        LoadRoom(currentRoomIndex);
    }

    private void LoadRoom(int index)
    {
        // 1. Cleanup old room
        if (currentRoomInstance != null)
        {
            Destroy(currentRoomInstance);
        }

        // 2. Spawn new room
        currentRoomInstance = Instantiate(roomPrefabs[index], Vector3.zero, Quaternion.identity);

        // 3. Teleport Player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Reset velocity to prevent carrying momentum between stages
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;

            Transform spawnPoint = currentRoomInstance.transform.Find("EntranceSpawnPoint");
            if (spawnPoint != null)
            {
                player.transform.position = spawnPoint.position;
            }
        }

        // 4. Update Camera through RoomController
        RoomController rc = currentRoomInstance.GetComponent<RoomController>();
        if (rc != null)
        {
            rc.ActivateRoom();
        }
    }
}