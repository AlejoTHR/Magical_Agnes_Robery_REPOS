using UnityEngine;

public class EnemigoSimple : MonoBehaviour
{
    public float velocidad = 2f;
    public float distancia = 5f;

    private Vector3 posicionInicial;
    private bool moviendoDerecha = true;

    void Start()
    {
        posicionInicial = transform.position;
    }

    void Update()
    {
        if (moviendoDerecha)
        {
            transform.Translate(Vector2.right * velocidad * Time.deltaTime);

            if (transform.position.x >= posicionInicial.x + distancia)
                moviendoDerecha = false;
        }
        else
        {
            transform.Translate(Vector2.left * velocidad * Time.deltaTime);

            if (transform.position.x <= posicionInicial.x - distancia)
                moviendoDerecha = true;
        }
    }
}
