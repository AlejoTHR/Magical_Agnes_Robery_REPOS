using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class Camera_Controller_Structure : MonoBehaviour
{    
    [Header("Target Game Object, It becomes a Collider [[ See CameraReference ]]")]
    public GameObject ColliderTarget = null;

    [Header("Target Game Object is the Player")]
    public GameObject Player;

    [Header("Canera Object")]
    public Camera CAMERA;


    [Header("Lerp Transition Speed")]
    public float speed = 1.0f;

    [Header("Sets an Offset in Camera's Position if needed")]
    public Vector3 offset = Vector3.zero;

    [Tooltip("Camera's initial Position")]
    public Vector3 baseCamPosition = Vector3.zero;
    Vector3 FinalTarget;

    [Header("Movement Script for _grounded")]
    public Movement _movement;

    [Header("Camera Boundaries")]
    public float minX = -1;
    public float minY = -1;
    public float maxX = -1;
    public float maxY = -1;

    public float aspectRatio;
    public float sizeX;
    public float sizeY;


    void Start()
    {
        FinalTarget = Vector3.zero;


        // If target has an asigned Game object
        if (ColliderTarget != null)
        { 
            baseCamPosition = ColliderTarget.transform.position;
            transform.position = baseCamPosition;
        }// Camera Initial Position = Target position
    }


    void Update()
    {
        // If target has an asigned Game object
        if (ColliderTarget != null || Player != null)
        {
            if (ColliderTarget.gameObject == null) return; 
            
            // STATIC CAMERA
            if (ColliderTarget.gameObject.CompareTag("ChangeCamera"))
            {
                FinalTarget = ColliderTarget.transform.position;
                // CAMERA MOVES TOWARDS OBJECTIVE
               
                baseCamPosition = Vector3.Lerp(baseCamPosition, FinalTarget + offset, speed * Time.deltaTime);
               
            }

            // VERTICAL SCROLL
            else if(ColliderTarget.gameObject.CompareTag("VerticalScroll"))
            {

                FinalTarget = new Vector3 (ColliderTarget.transform.position.x, Player.transform.position.y);

                // LIMITS
                if (maxX != -1 && maxY != -1 && minX != -1 && minY != -1)
                {
                    aspectRatio = 1.78f;
                    sizeX = CAMERA.orthographicSize/2;
                    sizeY = sizeX * aspectRatio;

                    baseCamPosition.y = Mathf.Clamp(baseCamPosition.y, minY + sizeY, maxY - sizeY);
                }


                baseCamPosition = Vector3.Lerp(baseCamPosition, FinalTarget + offset, speed * Time.deltaTime);

            }

            // HORIZONTAL SCROLL
            else if (ColliderTarget.gameObject.CompareTag("HorizontalScroll"))
            {

                /// if (ColliderTarget.transform.localScale.x + ColliderTarget.transform.localScale.x / 2 - CAMERA.orthographicSize >= CAMERA.orthographicSize)
                /// { // DEBUG IF
                ///     prueba = false;
                /// }
                //  // SCALE OFFSET FOR LIMITS
                //  scaleOffset = ColliderTarget.transform.localScale.x + ColliderTarget.transform.localScale.x / 2 - CAMERA.orthographicSize;
                //  
                //  // CAMERA COLLIDER SCALE FOR MESUREMENT
                //  scaleOffsetVector = ColliderTarget.transform.localScale /2;  


                // CAMERA COLLIDER FINAL TARGERT
                FinalTarget = new Vector3(Player.transform.position.x, ColliderTarget.transform.position.y);


                // CAMERA MOVES TOWARDS OBJECTIVE
                baseCamPosition = Vector3.Lerp(baseCamPosition, FinalTarget +offset, speed * Time.deltaTime);

                // LIMITS
                if (maxX != -1 && maxY != -1 && minX != -1 && minY != -1)
                {
                    aspectRatio = 1.78f; 
                    sizeY = CAMERA.orthographicSize;
                    sizeX = sizeY * aspectRatio;

                    baseCamPosition.x = Mathf.Clamp(baseCamPosition.x, minX + sizeX, maxX - sizeX);
                }
            }




            // CAMERA LOCKS INTO THE OBJECTIVE
            // Makes a transition to the next Collided Object tagged by ChangeCamera [[ See CameraReference ]]
            transform.position = baseCamPosition;

        }



    }
}
