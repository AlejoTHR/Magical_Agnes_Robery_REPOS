

using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(PolygonCollider2D))]
public class RoomController : MonoBehaviour
{
    [Header("Cinemachine Settings")]
    public CinemachineCamera vCam;
    [SerializeField] private int activePriority = 20;

    private void Start()
    {
        if (vCam != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                vCam.Follow = player.transform;
            }
        }
    }

    public void ActivateRoom()
    {
        if (vCam != null)
        {
            vCam.Priority = activePriority;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ActivateRoom();
        }
    }
}