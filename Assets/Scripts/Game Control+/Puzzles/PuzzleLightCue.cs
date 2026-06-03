using UnityEngine;

public class PuzzleLightCue : MonoBehaviour
{
    public string lightID;

    [Header("Light")]
    public Sprite spriteOff;
    public Sprite spriteOn;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = spriteOff;
    }

    public void ActivateLight(string incomingID)
    {
        if (incomingID == lightID)
        {
            sr.sprite = spriteOn;
        }
    }
}