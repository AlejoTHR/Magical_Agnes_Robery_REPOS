using UnityEngine;

public class Mov_Structure_Temp : MonoBehaviour
{
    [Header("Mov Structure For Following Camera")]
    [Tooltip("Object Speed")]
    public float speed = 50.0f;

    //public Hide_Structuretmp HideStructure;
    //public GameObject Hide = null;

    public bool IsHidden = false;


    void Update()
    {
        int movementX = 0;
        int movementY = 0;
        if(IsHidden == false)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                movementY = 1;
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                movementX = -1;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                movementX = 1;
            }
            
            if (Input.GetKey(KeyCode.F) )//&& Hide.CompareTag("Hide") && Hide != null)
            {
                IsHidden = true;
            }
            

            transform.position = transform.position + new Vector3(movementX, movementY, 0) * speed * Time.deltaTime;
        }
        else
        {

            if (!Input.GetKey(KeyCode.F))
            {
                IsHidden = false;
            }
        }


    }
}
