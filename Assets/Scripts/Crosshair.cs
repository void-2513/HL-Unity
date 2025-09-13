using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [Header("Crosshair Settings")]
    public Color crosshairColor = new Color(0.88f, 0.91f, 0.44f, 1f); // HL2's distinctive yellow-green
    public float size = 32f; // Total size of the crosshair
    public float thickness = 2f; // Line thickness
    public float gap = 4f; // Center gap size

    private Texture2D crosshairTexture;
    private GUIStyle crosshairStyle;

    void Start()
    {
        CreateCrosshairTexture();
    }

    void CreateCrosshairTexture()
    {
        int texSize = Mathf.CeilToInt(size);
        crosshairTexture = new Texture2D(texSize, texSize, TextureFormat.ARGB32, false);
        
        // Make texture transparent by default
        Color[] pixels = new Color[texSize * texSize];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.clear;
        }
        crosshairTexture.SetPixels(pixels);
        
        // Calculate center
        int center = texSize / 2;
        int halfThickness = Mathf.CeilToInt(thickness / 2f);
        int gapPixels = Mathf.CeilToInt(gap / 2f);
        
        // Draw horizontal line
        for (int x = 0; x < texSize; x++)
        {
            if (x < center - gapPixels || x > center + gapPixels)
            {
                for (int y = center - halfThickness; y <= center + halfThickness; y++)
                {
                    if (y >= 0 && y < texSize)
                    {
                        crosshairTexture.SetPixel(x, y, crosshairColor);
                    }
                }
            }
        }
        
        // Draw vertical line
        for (int y = 0; y < texSize; y++)
        {
            if (y < center - gapPixels || y > center + gapPixels)
            {
                for (int x = center - halfThickness; x <= center + halfThickness; x++)
                {
                    if (x >= 0 && x < texSize)
                    {
                        crosshairTexture.SetPixel(x, y, crosshairColor);
                    }
                }
            }
        }
        
        crosshairTexture.Apply();
        
        // Create GUIStyle
        crosshairStyle = new GUIStyle();
        crosshairStyle.normal.background = crosshairTexture;
    }

    void OnGUI()
    {
        if (crosshairTexture == null || crosshairStyle == null)
            return;
            
        // Draw crosshair at center of screen
        float x = (Screen.width - size) / 2;
        float y = (Screen.height - size) / 2;
        
        GUI.Label(new Rect(x, y, size, size), "", crosshairStyle);
    }

    void OnValidate()
    {
        // Recreate texture when values change in inspector
        if (crosshairTexture != null)
        {
            CreateCrosshairTexture();
        }
    }

    void OnDestroy()
    {
        // Clean up
        if (crosshairTexture != null)
        {
            DestroyImmediate(crosshairTexture);
        }
    }
}
