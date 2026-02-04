using UnityEngine;

public class ElectricMagic : MonoBehaviour
{
    public Rigidbody2D agnes;
    private bool markerPlaced = false;
    private Vector3 markerPosition;
    private Movement plymov = null;

    private void Start()
    {
        plymov = GetComponent<Movement>();
    }

    private void Update()
    {
        if (plymov == null) return;
        HandleMarkerInput();
    }


    private void HandleMarkerInput()
    {
        if (Input.GetKeyDown(KeyCode.Z) && !markerPlaced)
        {
            markerPosition = agnes.transform.position;
            markerPlaced = true;

            Debug.Log("Marcador colocado en: " + markerPosition);
        }

        else if (Input.GetKeyDown(KeyCode.Z) && markerPlaced)
        {
            TeleportToMarker();
        }
    }

    private void TeleportToMarker()
    {
        agnes.transform.position = markerPosition;

        plymov.SetFrameVelocity(Vector2.zero);

        markerPlaced = false;

        Debug.Log("Teletransportado al marcador");
    }
}