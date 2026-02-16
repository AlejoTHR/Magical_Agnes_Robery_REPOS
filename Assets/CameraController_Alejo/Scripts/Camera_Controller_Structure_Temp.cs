using UnityEngine;

public class Camera_Controller_Structure_Temp : MonoBehaviour
{
    [Header("Camera Movement Structure")]
    
    [Tooltip("Target Game Object, It becomes a Collider [[ See CameraReference ]]")]
    public GameObject ColliderTarget = null;

    [Tooltip("Target Game Object is the Player")]
    public GameObject Player;


    [Tooltip("Lerp Transition Speed")]
    public float speed = 1.0f;

    [Tooltip("Sets an Offset in Camera's Position if needed")]
    public Vector3 offset = Vector3.zero;

    [Tooltip("Camera's initial Position")]
    public Vector3 baseCamPosition = Vector3.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
            Vector3 FinalTarget = Vector3.zero;
            if (ColliderTarget.gameObject == null) { return; }
            if (ColliderTarget.gameObject.tag == "ChangeCamera")
            {
                FinalTarget = ColliderTarget.transform.position;
            }
            else if(ColliderTarget.gameObject.tag == "VerticalScroll")
            {
                FinalTarget = new Vector3 (ColliderTarget.transform.position.x, 
                    Player.transform.position.y);
            }
            else if (ColliderTarget.gameObject.tag == "HorizontalScroll")
            {
                FinalTarget = new Vector3(Player.transform.position.x,
                    ColliderTarget.transform.position.y);
            }

            baseCamPosition = Vector3.Lerp(baseCamPosition, FinalTarget, speed * Time.deltaTime);
            
            
            
            transform.position = baseCamPosition + offset;
            // Makes a transition to the next Collided Object tagged by ChangeCamera [[ See CameraReference ]]
        }



    }
}
