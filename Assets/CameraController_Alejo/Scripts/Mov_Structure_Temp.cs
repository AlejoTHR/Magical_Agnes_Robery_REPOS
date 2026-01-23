using UnityEngine;

public class Mov_Structure_Temp : MonoBehaviour
{
    [Header("Mov Structure For Following Camera")]
    [Tooltip("Object Speed")]
    public float speed = 50.0f;


    void Update()
    {
        int movementX = 0;
        int movementY = 0;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            movementX = -1;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            movementX = 1;
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            movementY = 1;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            movementY = -1;
        }

        transform.position = transform.position + new Vector3(movementX, movementY, 0) * speed * Time.deltaTime;

        /***************

        CHANGE FOR FINAL DESSGINED MOVEMENT SETTING

        ***************/

    }
}
