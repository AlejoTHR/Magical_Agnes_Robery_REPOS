using TreeEditor;
using UnityEngine;

public class Enemy_mov_TMP : MonoBehaviour
{

    public Vector3 initialPos;
    public float PosR = 5;
    public float PosL = -5;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.x < initialPos.x + PosR)
        {
            transform.position += Vector3.right * Time.deltaTime;
        }

    }

    public void RESET_ENEMIES()
    {
        transform.position = initialPos;
    }


}
