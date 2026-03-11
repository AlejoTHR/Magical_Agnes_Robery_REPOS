using UnityEngine;

public class Teleporter : MonoBehaviour
{

    private bool markerPlaced = false;
    private Vector3 markerPosition;


    private void TeleportToMarker()
    {
        transform.position = markerPosition;

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.linearVelocity = Vector2.zero;

        Debug.Log("Teletransportado al marcador");
    }

    private void HandleMarkerInput()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            markerPosition = transform.position;
            markerPlaced = true;

            Debug.Log("Marcador colocado en: " + markerPosition);
        }

        if (Input.GetKeyDown(KeyCode.K) && markerPlaced)
        {
            TeleportToMarker();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        HandleMarkerInput();
    }
}



