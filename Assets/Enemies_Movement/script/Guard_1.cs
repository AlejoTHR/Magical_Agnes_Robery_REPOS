using UnityEngine;

public class Guard_1 : MonoBehaviour
{
    public float Timer = 0f;
    public float TimeLimit1 = 0;
    public float TimeLimit2 = 0;
    public Sprite Sprite1;
    public Sprite Sprite2;
    SpriteRenderer m_SpriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_SpriteRenderer.sprite = Sprite1;
    }

    // Update is called once per frame
    void Update()
    {
        Timer++;

        if (Timer > TimeLimit1 && m_SpriteRenderer.sprite == Sprite1)
        {
            m_SpriteRenderer.sprite = Sprite2;
            Timer = 0f;
        }
        if (Timer > TimeLimit2 && m_SpriteRenderer.sprite == Sprite2)
        {
            m_SpriteRenderer.sprite = Sprite1;
            Timer = 0f;
        }
    }
}
