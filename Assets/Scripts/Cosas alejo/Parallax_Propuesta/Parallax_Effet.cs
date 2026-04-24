using UnityEngine;

public class Parallax_Effet : MonoBehaviour
{
    [Header("Components")]
    public Transform cam;

    [Header("Paarllax Atributes")]
    public Vector3 camStartpos;
    public float distance;
    public float farthestBg;

    [Range(0.01f, 0.05f)] public float parallaxSpeed;


    [Header("Objects Arrays")]
    public GameObject[] Backgrounds;
    public Material[] mat;
    public float[] bgSpeed;




    private void Start()
    {
        camStartpos = cam.position; // ALL BACKGROUNDS FOLLOWS THE CAMERA

        int bgCount = transform.childCount; // COUNTS ALL CHILDREN GAMEOBJECTS OF THE BACKGROUNDS FATHER
        mat = new Material[bgCount]; // MATERIAL FOR BACKGROUN LOOPER
        bgSpeed = new float[bgCount]; // SPEED OF EACH LAYR
        Backgrounds = new GameObject[bgCount]; // LAYER GAME OBJECT

        for (int i = 0; i < bgCount; i++)
        {
            Backgrounds[i] = transform.GetChild(i).gameObject; // Background GAMEOBJEC ARRAY IS FILLIED BY EACH CHILDREN
            mat[i] = Backgrounds[i].GetComponent<Renderer>().material; // mat ARRAY GETS THE LAYER'S MATERIAL FOR RENDERING
        }
        BgSpeedCalculation(bgCount);
    }


    void BgSpeedCalculation(int bgCount)
    {
        for (int i = 0; i < bgCount; i++)
        {
            if ((Backgrounds[i].transform.position.z - cam.position.z) > farthestBg) // IF Z PSOTION AND CAM POSITION IS BELOW THE FARTHEST
            {
                farthestBg = Backgrounds[i].transform.position.z - cam.position.z; // BACKGROUND GAMEOBJECT BECOMES THE FARTHEST YALER
            }

        }

        for (int i = 0; i < bgCount; i++)
        {// SPEED EQUALS 1 - LAYER POSITION / FARTHEST BACKGROUND ASSIGNED
            bgSpeed[i] = 1 - (Backgrounds[i].transform.position.z - cam.position.z) / farthestBg;
        }
    }

    private void LateUpdate()
    {
        distance = cam.position.x - camStartpos.x; // DISTANCE TO INITiAL POSITION AND AMERA ACTUAL POSITION
        transform.position = new Vector3(cam.position.x, transform.position.y, 0); // PARALLAX POSITION IS THE CAM X AXIS AND THIS Y AXIS

        for (int i = 0; i < Backgrounds.Length; i++)
        {
            float speed = bgSpeed[i] * parallaxSpeed; // BACKGROUND SPEED + PARALLAX SPEED APPLIED IN EDITOR
            mat[i].SetTextureOffset("_MainTex", new Vector2(distance, 0) * speed); //´MOVES THE PARALLAX's TEXTURE
        }

    }

}
