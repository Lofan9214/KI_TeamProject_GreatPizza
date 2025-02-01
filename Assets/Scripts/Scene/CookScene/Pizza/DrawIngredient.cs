using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DrawIngredient : MonoBehaviour
{
    public Texture2D brushTexture;
    public Texture2D spriteTexture;

    private int textureHeight = 256;
    private int textureWidth = 256;
    private int brushHeight = 4;
    private int brushWidth = 4;

    private Color[] brushColorMap;
    private float[] spriteAlphaMap;
    private Color[] drawColorMap;
    private float[] drawAlphaMap;

    private Texture2D drawTexture;

    public float Ratio => drawAlphaMap.Sum() / spriteAlphaMap.Sum();


    private void Start()
    {
        textureHeight = spriteTexture.height;
        textureWidth = spriteTexture.width;

        drawColorMap = spriteTexture.GetPixels();
        spriteAlphaMap = new float[textureHeight * textureWidth];
        drawAlphaMap = new float[textureHeight * textureWidth];

        for (int i = 0; i < drawColorMap.Length; ++i)
        {
            spriteAlphaMap[i] = drawColorMap[i].a;
            drawColorMap[i].a = 0f;
        }

        drawTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.ARGB32, false);

        SetTexture();
        var renderer = GetComponent<SpriteRenderer>();
        renderer.sprite = Sprite.Create(drawTexture, new Rect(0f, 0f, drawTexture.width, drawTexture.height), Vector2.one * 0.5f, textureHeight);
        renderer.sprite.name = "DrawLayer";

        brushHeight = brushTexture.height;
        brushWidth = brushTexture.width;
        brushColorMap = brushTexture.GetPixels();
    }

    private void SetTexture()
    {
        drawTexture.SetPixels(drawColorMap);
        drawTexture.Apply();
    }

    private void DrawBrush(Vector3 pos)
    {
        int xStart = Mathf.RoundToInt(pos.x * textureWidth - brushWidth * 0.5f);
        int xEnd = Mathf.RoundToInt(pos.x * textureWidth + brushWidth * 0.5f);
        int yStart = Mathf.RoundToInt(pos.y * textureHeight - brushHeight * 0.5f);
        int yEnd = Mathf.RoundToInt(pos.y * textureHeight + brushHeight * 0.5f);

        for (int i = xStart, ii = 0; i < xEnd; ++i, ++ii)
        {
            for (int j = yStart, jj = 0; j < yEnd; ++j, ++jj)
            {
                if (i < 0 || i >= textureWidth
                    || j < 0 || j >= textureHeight)
                {
                    continue;
                }
                float alpha = drawAlphaMap[j * textureWidth + i] + brushColorMap[jj * brushWidth + ii].a;
                alpha = Mathf.Min(alpha, spriteAlphaMap[j * textureWidth + i]);
                drawColorMap[j * textureWidth + i].a = alpha;
                drawAlphaMap[j * textureWidth + i] = alpha;
            }
        }
    }

    public void DrawPoint(Vector2 point)
    {
        var localpos = transform.InverseTransformPoint(point);
        localpos += Vector3.one * 0.5f;
        DrawBrush(localpos);
        SetTexture();
    }
}
