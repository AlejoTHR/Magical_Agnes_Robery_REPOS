using UnityEngine;
using System.Collections.Generic;

public class ParallaxManager : MonoBehaviour
{
    public enum DepthLevel { Closest, Near, Mid, Far, Farthest }

    [System.Serializable]
    public class ParallaxLayer
    {
        public string layerName;
        public DepthLevel depth;
        public float yOffset;

        [Tooltip("Assign the 3 copies (Left, Center, Right)")]
        public GameObject[] backgroundParts = new GameObject[3];

        [HideInInspector] public float startPosX;
        [HideInInspector] public float startPosY;
        [HideInInspector] public float lengthX;
        [HideInInspector] public float halfHeight;
    }

    public Camera cam;

    [Header("Vertical Settings")]
    [Range(0f, 1f)]
    public float verticalFollowRate = 0.5f;

    public List<ParallaxLayer> layers = new List<ParallaxLayer>();

    private void Start()
    {
        if (cam == null) cam = Camera.main;

        foreach (var layer in layers)
        {
            if (layer.backgroundParts.Length > 0 && layer.backgroundParts[0] != null)
            {
                SpriteRenderer sprite = layer.backgroundParts[0].GetComponent<SpriteRenderer>();
                layer.lengthX = sprite.bounds.size.x;
                layer.halfHeight = sprite.bounds.extents.y;

                // We start the layer exactly where the camera starts
                layer.startPosX = cam.transform.position.x;
                layer.startPosY = cam.transform.position.y + layer.yOffset;

                int order = GetSortingOrder(layer.depth);

                for (int i = 0; i < layer.backgroundParts.Length; i++)
                {
                    // Snap the 3 parts to be perfectly tiled: Left (-1), Center (0), Right (1)
                    // This removes dependency on initial scene placement
                    float offset = (i - 1) * layer.lengthX;
                    layer.backgroundParts[i].transform.position = new Vector3(layer.startPosX + offset, layer.startPosY, layer.backgroundParts[i].transform.position.z);

                    var renderer = layer.backgroundParts[i].GetComponent<SpriteRenderer>();
                    if (renderer != null) renderer.sortingOrder = order;
                }
            }
        }
    }

    private void Update()
    {
        if (cam == null) return;

        Vector3 camPos = cam.transform.position;

        foreach (var layer in layers)
        {
            float pEffect = GetEffectValue(layer.depth);

            // How far we have moved relative to the world
            float distance = camPos.x * pEffect;

            // How much the "virtual" background has moved (for the jump logic)
            float temp = camPos.x * (1 - pEffect);

            // --- IRONCLAD VERTICAL CALCULATION ---

            // 1. Calculate the raw target position
            float targetY = layer.startPosY + (camPos.y * verticalFollowRate * pEffect);

            // 2. Add a tiny bit of "safety padding" (e.g., 0.1 units) 
            // to account for pixel-rounding gaps
            float safetyPadding = 0.1f;

            // 3. Define the absolute limits targetY can reach before edges show
            // These calculations ensure the BG never moves enough to show the camera the void
            float minTargetY = camPos.y - layer.halfHeight + cam.orthographicSize + safetyPadding;
            float maxTargetY = camPos.y + layer.halfHeight - cam.orthographicSize - safetyPadding;

            // 4. Tight Clamp: This forces targetY to stay within the bounds of the sprite
            if (layer.halfHeight * 2 > cam.orthographicSize * 2)
            {
                targetY = Mathf.Clamp(targetY, minTargetY, maxTargetY);
            }
            else
            {
                // Fallback: If sprite is somehow shorter than the camera, center it perfectly
                targetY = camPos.y;
            }
            // Update the positions of the 3 tiled parts
            for (int i = 0; i < layer.backgroundParts.Length; i++)
            {
                if (layer.backgroundParts[i] != null)
                {
                    float seamBuffer = 0.02f;
                    float xPos = layer.startPosX + distance + ((i - 1) * (layer.lengthX - seamBuffer));
                    layer.backgroundParts[i].transform.position = new Vector3(xPos, targetY, layer.backgroundParts[i].transform.position.z);
                }
            }

            // --- THE CONSISTENT JUMP ---
            // If the camera has moved more than one full length past the anchor, 
            // shift the anchor so the "Center" piece is always roughly under the camera.
            if (temp > layer.startPosX + (layer.lengthX / 2))
                layer.startPosX += layer.lengthX;
            else if (temp < layer.startPosX - (layer.lengthX / 2))
                layer.startPosX -= layer.lengthX;
        }
    }

    private float GetEffectValue(DepthLevel depth)
    {
        switch (depth)
        {
            case DepthLevel.Closest: return 0.2f;  // Closer moves faster
            case DepthLevel.Near: return 0.5f;
            case DepthLevel.Mid: return 0.7f;
            case DepthLevel.Far: return 0.9f;
            case DepthLevel.Farthest: return 0.98f; // Moves slowest (almost follows camera)
            default: return 0.5f;
        }
    }

    private int GetSortingOrder(DepthLevel depth)
    {
        switch (depth)
        {
            case DepthLevel.Closest: return -10;
            case DepthLevel.Near: return -20;
            case DepthLevel.Mid: return -30;
            case DepthLevel.Far: return -40;
            case DepthLevel.Farthest: return -50;
            default: return -100;
        }
    }
}