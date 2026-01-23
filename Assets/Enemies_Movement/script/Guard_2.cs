using UnityEngine;

public class Guard_2: MonoBehaviour
{
    public float Timer = 0f;
    public float TimeLimit = 0;
    SpriteRenderer m_SpriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Timer++;

        if (Timer > TimeLimit)
        {
            m_SpriteRenderer.flipX = !m_SpriteRenderer.flipX;
            Timer = 0f;
        }
    }
}
