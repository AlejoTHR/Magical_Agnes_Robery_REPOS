using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TMP_Wave : MonoBehaviour
{
    [Header("Wave Settings")]
    public float bounceAmount = 5f;
    public float speed = 8f;
    public float waveOffset = 0.5f;

    [HideInInspector] public bool isHovered = false;

    private TextMeshProUGUI m_TextComponent;
    private TMP_MeshInfo[] cachedMeshInfo;

    void Awake() => m_TextComponent = GetComponent<TextMeshProUGUI>();

    void LateUpdate()
    {
        if (!isHovered) return;

        m_TextComponent.ForceMeshUpdate();
        TMP_TextInfo textInfo = m_TextComponent.textInfo;

        if (cachedMeshInfo == null || cachedMeshInfo.Length != textInfo.meshInfo.Length)
            cachedMeshInfo = textInfo.CopyMeshInfoVertexData();

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int matIndex = charInfo.materialReferenceIndex;
            int vIndex = charInfo.vertexIndex;

            Vector3[] sourceVertices = cachedMeshInfo[matIndex].vertices;
            Vector3[] destVertices = textInfo.meshInfo[matIndex].vertices;

            // FIX: Use unscaledTime so it animates while Time.timeScale is 0
            float yOffset = Mathf.Sin(Time.unscaledTime * speed + (i * waveOffset)) * bounceAmount;
            Vector3 offset = new Vector3(0, yOffset, 0);

            for (int j = 0; j < 4; j++)
                destVertices[vIndex + j] = sourceVertices[vIndex + j] + offset;
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            m_TextComponent.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }

    public void ResetText()
    {
        isHovered = false;
        if (m_TextComponent != null) m_TextComponent.ForceMeshUpdate();
    }
}