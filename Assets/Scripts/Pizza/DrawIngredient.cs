using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class DrawIngredient : MonoBehaviour
{
    private readonly string fireAxis = "Fire1";
    private Color brushColor;

    public int textureHeight = 256;
    public int textureWidth = 256;
    public int brushSize = 4;

    public Texture2D sourceTexture;

    Color[] colorMap;

    private void Start()
    {
        colorMap = sourceTexture.GetPixels();
        textureHeight = sourceTexture.height;
        textureWidth = sourceTexture.width;
        for (int i = 0; i < colorMap.Length; ++i)
        {
            colorMap[i] = Color.white;
            colorMap[i].a = 0f;
        }
        SetTexture();

        

        
    }

    private void Update()
    {
        if (Input.GetButton(fireAxis))
        {
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var localpos = transform.InverseTransformPoint(pos) + 0.5f * Vector3.one;
            DrawBrush(localpos.x, localpos.y);
            SetTexture();
        }
    }

    private void CalculatePixel()
    {

    }

    private void SetTexture()
    {
        sourceTexture.SetPixels(colorMap);
        sourceTexture.Apply();
    }

    private void DrawBrush(float x, float y)
    {
        int xStart = Mathf.Max(Mathf.RoundToInt(x * 256 - brushSize), 0);
        int xEnd = Mathf.Min(Mathf.RoundToInt(x * 256 + brushSize), textureWidth - 1);
        int yStart = Mathf.Max(Mathf.RoundToInt(y * 256 - brushSize), 0);
        int yEnd = Mathf.Min(Mathf.RoundToInt(y * 256 + brushSize), textureHeight - 1);

        for (int i = xStart; i <= xEnd; ++i)
        {
            for (int j = yStart; j <= yEnd; ++j)
            {
                colorMap[j * textureWidth + i].a = 1f;
            }
        }
    }
}
