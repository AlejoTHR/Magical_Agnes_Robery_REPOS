using UnityEngine;

public class CameraReference : MonoBehaviour
{
    [Tooltip("Camera Script Object for Reference")]
    public Camera_Controller_Structure cameraRef = null;

    [Header("Camera")]
    public GameObject cam;

    [Header("Enemy Script")]
    public Enemy_mov_TMP enemy_move;

    private void OnTriggerEnter2D(Collider2D collided)
    {
        cam = collided.gameObject;
        /// transform.scale.x + transform.scale.x/2 - Tamaño de la Cámara en x (HACERLO EN TODOS LOS LADOS)

        // If collition is detected and This object has a Camera Reference Game Object
        if (collided != null && cameraRef != null)
        {
            // If Game Object Tag is equal to the Bg Tag
            if (collided.gameObject.tag == "ChangeCamera" || collided.gameObject.tag == "VerticalScroll" || collided.gameObject.tag == "HorizontalScroll")
            {
                cameraRef.ColliderTarget = collided.gameObject;

                enemy_move.RESET_ENEMIES();

            }

        }
    }


}
