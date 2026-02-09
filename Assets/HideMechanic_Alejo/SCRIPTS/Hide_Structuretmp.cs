using UnityEngine;

public class Hide_Structuretmp : MonoBehaviour
{
    public Movement Mov_Structure;

    private void OnTriggerEnter2D(Collider2D Hide_Collisioned)
    {
        if(Hide_Collisioned != null)
        {
            if(Hide_Collisioned.gameObject.CompareTag("Hide"))
            {
                Mov_Structure.Hide = Hide_Collisioned.gameObject;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D Exit_Hide)
    {
        if (Exit_Hide.gameObject.tag == "Hide")
        {
            Mov_Structure.Hide = null;
        }
    }




}
