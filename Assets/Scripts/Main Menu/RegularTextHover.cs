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
    [Range(0.1f, 5f)][SerializeField] private float _colorFrequency = 1.0f; // How "squished" the rainbow is
    [SerializeField] private float _colorScrollSpeed = 0.5f;             // Speed of the gradient scroll

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

            // 1. Calculate Wave Position
            float yOffset = Mathf.Sin(Time.unscaledTime * speed + (i * waveOffset)) * bounceAmount;
            Vector3 offset = new Vector3(0, yOffset, 0);

            // 2. Apply Wave and Calculate Colors
            for (int j = 0; j < 4; j++)
            {
                // Apply wave
                destVertices[vIndex + j] = sourceVertices[vIndex + j] + offset;

                // 3. APPLY RAINBOW (New Section)
                if (_enableRainbow)
                {
                    // Get a unique value for this vertex based on its X position
                    // We use localPosition or worldPosition to create the spatial gradient.
                    // World position makes the rainbow feel like a stationary light source.
                    // Local position (destVertices[vIndex+j].x) makes it follow the text.
                    float hueSource = destVertices[vIndex + j].x + transform.position.x;

                    // Calculate Hue (H) based on space + time
                    // We multiply by colorFrequency to determine how many 'rainbows' appear per unit of distance
                    float hue = (hueSource * (_colorFrequency * 0.01f)) + (Time.unscaledTime * _colorScrollSpeed);

                    // Normalize hue to 0-1 range
                    hue = hue % 1.0f;
                    if (hue < 0) hue += 1.0f;

                    // Convert HSV to RGB color (Saturation 1, Value 1)
                    Color32 rainbowColor = Color.HSVToRGB(hue, 1.0f, 1.0f);

                    // Grab the color array from the mesh
                    Color32[] destColors = textInfo.meshInfo[matIndex].colors32;
                    // Apply color to the vertex
                    destColors[vIndex + j] = rainbowColor;
                }
            }
        }

        // Apply geometry changes
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;

            // NEW: Must also apply color changes back to the mesh
            if (_enableRainbow)
            {
                textInfo.meshInfo[i].mesh.colors32 = textInfo.meshInfo[i].colors32;
            }

            m_TextComponent.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}