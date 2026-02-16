using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ElectricMagic : MonoBehaviour
{
    public Rigidbody2D agnes;
    private bool markerPlaced = false;
    private Vector3 markerPosition;
    private Movement plymov = null;
    public GameObject eletrik;
    private GameObject clone;

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
        if (Input.GetKeyDown(KeyCode.C) && !markerPlaced)
        {
            markerPosition = agnes.transform.position;
            markerPlaced = true;
            clone = Instantiate(eletrik, transform.position, transform.rotation);
            Debug.Log("Marcador colocado en: " + markerPosition);
            
        }
        else if (Input.GetKeyDown(KeyCode.P) && markerPlaced)
        {
            TeleportToMarker();
            Destroy(clone);
             plymov.isHiding = false;

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