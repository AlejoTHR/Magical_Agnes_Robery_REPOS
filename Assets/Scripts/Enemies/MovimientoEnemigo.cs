using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.RuleTile.TilingRuleOutput;


public class MovimientoEnemigo : MonoBehaviour
{
    #region Public Variables
    public float speed;
    public float leftLimit;
    public float rightLimit;
    #endregion

    #region Private Variables
    private Animator anim;
    private int direction = 1;
    private SpriteRenderer spriteRenderer;
    #endregion

    private void Awake()
    {
        //anim = GetComponent<Animator>();
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        //anim.SetBool("Walk", true);
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);

        // Si llega al límite derecho, cambia a la izquierda
        if (transform.position.x >= rightLimit)
        {
            direction = -1;
            Flip();
        }
        // Si llega al límite izquierdo, cambia a la derecha
        else if (transform.position.x <= leftLimit)
        {
            direction = 1;
            Flip();
        }
    }

    void Flip()
    {
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }
}
