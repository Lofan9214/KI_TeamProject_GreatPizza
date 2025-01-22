using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using static UnityEditor.PlayerSettings;

public class DrawIngredient : MonoBehaviour
{
    public int textureHeight = 256;
    public int textureWidth = 256;
    public int brushHeight = 4;
    public int brushWidth = 4;

    public Texture2D brushTexture;
    public Texture2D maskTexture;
    private SpriteMask spriteMask;

    private Color[] maskColorMap;
    private Color[] brushColorMap;

    private void Awake()
    {
        spriteMask = GetComponent<SpriteMask>();
    }

    private void Start()
    {
        maskColorMap = new Color[textureHeight * textureWidth];

        for (int i = 0; i < maskColorMap.Length; ++i)
        {
            maskColorMap[i] = Color.white;
            maskColorMap[i].a = 0f;
        }

        maskTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.ARGB32, false);
        maskTexture.filterMode = FilterMode.Point;
        SetTexture();
        spriteMask.sprite = Sprite.Create(maskTexture, new Rect(0f, 0f, maskTexture.width, maskTexture.height), Vector2.one * 0.5f, textureHeight);
        spriteMask.sprite.name = "MaskTexture";

        brushHeight = brushTexture.height;
        brushWidth = brushTexture.width;
        brushColorMap = brushTexture.GetPixels();
    }

    private void SetTexture()
    {
        maskTexture.SetPixels(maskColorMap);
        maskTexture.Apply();
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
                float alpha = maskColorMap[j * textureWidth + i].a + brushColorMap[jj * brushWidth + ii].a;
                maskColorMap[j * textureWidth + i].a = Mathf.Min(alpha, 1f);
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
