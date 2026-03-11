using UnityEngine;

public class CameraReference : MonoBehaviour
{
    [Tooltip("Camera Script Object for Reference")]
    public Camera_Controller_Structure_Temp cameraRef = null;


    private void OnTriggerEnter2D(Collider2D collided)
    {
        // If collition is detected and This object has a Camera Reference Game Object
        if (collided != null && cameraRef != null)
        {
            // If Game Object Tag is equal to the Bg Tag
            if (collided.gameObject.tag == "ChangeCamera" 
                || collided.gameObject.tag == "VerticalScroll" 
                || collided.gameObject.tag == "HorizontalScroll")
            {
                cameraRef.ColliderTarget = collided.gameObject;
            }

            //else if(collided.gameObject.tag == "VerticalScroll")
            //{
            //    cameraRef.target = cameraRef.Player.gameObject;
            //}

        }
    }


}
