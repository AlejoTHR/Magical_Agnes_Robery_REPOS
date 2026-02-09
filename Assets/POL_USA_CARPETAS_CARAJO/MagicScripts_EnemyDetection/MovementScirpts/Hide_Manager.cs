using UnityEngine;

public class Hide_Manager : MonoBehaviour
{
    public GameObject HideCollided;

    private void OnTriggerEnter2D(Collider2D Hide_Collided)
    {
        if(HideCollided != null && Hide_Collided.gameObject.tag == "Hide")
        {
            HideCollided = Hide_Collided.gameObject;
        }
    }





}
