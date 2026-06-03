using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TMP_ConstantWave : MonoBehaviour
{
    [Header("Wave Settings")]
    public float bounceAmount = 5f;
    public float speed = 8f;
    public float waveOffset = 0.5f;

    [Header("Rainbow Settings")]
    [SerializeField] private bool _enableRainbow = true;
    [Range(0.1f, 5f)][SerializeField] private float _colorFrequency = 1.0f; 
    [SerializeField] private float _colorScrollSpeed = 0.5f;           

    private TextMeshProUGUI m_TextComponent;
    private TMP_MeshInfo[] cachedMeshInfo;

    void Awake()
    {
        m_TextComponent = GetComponent<TextMeshProUGUI>();
    }

    void OnEnable()
    {
        m_TextComponent.ForceMeshUpdate();
    }

    void LateUpdate()
    {
        m_TextComponent.ForceMeshUpdate();
        TMP_TextInfo textInfo = m_TextComponent.textInfo;

        if (cachedMeshInfo == null || cachedMeshInfo.Length != textInfo.meshInfo.Length)
        {
            cachedMeshInfo = textInfo.CopyMeshInfoVertexData();
        }

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;
            int matIndex = charInfo.materialReferenceIndex;
            int vIndex = charInfo.vertexIndex;
            Vector3[] sourceVertices = cachedMeshInfo[matIndex].vertices;
            Vector3[] destVertices = textInfo.meshInfo[matIndex].vertices;
            float yOffset = Mathf.Sin(Time.unscaledTime * speed + (i * waveOffset)) * bounceAmount;
            Vector3 offset = new Vector3(0, yOffset, 0);

            for (int j = 0; j < 4; j++)
            {
                destVertices[vIndex + j] = sourceVertices[vIndex + j] + offset;

                if (_enableRainbow)
                {
                    float hueSource = destVertices[vIndex + j].x + transform.position.x;
                    float hue = (hueSource * (_colorFrequency * 0.01f)) + (Time.unscaledTime * _colorScrollSpeed);
                    hue = hue % 1.0f;
                    if (hue < 0) hue += 1.0f;
                    Color32 rainbowColor = Color.HSVToRGB(hue, 1.0f, 1.0f);
                    Color32[] destColors = textInfo.meshInfo[matIndex].colors32;
                    destColors[vIndex + j] = rainbowColor;
                }
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            if (_enableRainbow)
            {
                textInfo.meshInfo[i].mesh.colors32 = textInfo.meshInfo[i].colors32;
            }
            m_TextComponent.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}