using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class DrawIngredient : MonoBehaviour
{
    public string IngredientId { get; private set; } = string.Empty;
    public Sprite[] layerSprites;
    public Sprite brushSprite;

    private int textureHeight = 256;
    private int textureWidth = 256;
    private int brushHeight = 4;
    private int brushWidth = 4;

    private Color[] brushColorMap;
    private float[] spriteAlphaMap;
    private Color[][] drawColorMap;
    private float[] drawAlphaMap;

    private Texture2D drawTexture;

    public float pixelsPerPoint = 100f;

    private int currentIndex = 0;

    public float Ratio => drawAlphaMap.Sum() / spriteAlphaMap.Sum();

    public void Init(string ingredientId)
    {
        IngredientId = ingredientId;
        layerSprites = DataTableManager.IngredientTable.Get(ingredientId).spriteDatas.toppingSprites;

        var rect = layerSprites[0].textureRect;
        textureHeight = (int)rect.height;
        textureWidth = (int)rect.width;

        if (!layerSprites[0].texture.isReadable)
        {
            var origTexPath = AssetDatabase.GetAssetPath(layerSprites[0].texture);
            var ti = (TextureImporter)AssetImporter.GetAtPath(origTexPath);
            ti.isReadable = true;
            ti.SaveAndReimport();
        }

        drawColorMap = new Color[layerSprites.Length][];
        for (int i = 0; i < layerSprites.Length; ++i)
        {
            rect = layerSprites[i].textureRect;
            drawColorMap[i] = layerSprites[i].texture.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
        }
        spriteAlphaMap = new float[textureHeight * textureWidth];
        drawAlphaMap = new float[textureHeight * textureWidth];

        for (int i = 0; i < drawColorMap[currentIndex].Length; ++i)
        {
            spriteAlphaMap[i] = drawColorMap[currentIndex][i].a;
            for (int j = 0; j < drawColorMap.Length; ++j)
            {
                drawColorMap[j][i].a = 0f;
            }
        }

        drawTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.ARGB32, false);

        SetTexture();
        var renderer = GetComponent<SpriteRenderer>();
        renderer.sprite = Sprite.Create(drawTexture, new Rect(0f, 0f, drawTexture.width, drawTexture.height), Vector2.one * 0.5f, pixelsPerPoint);
        renderer.sprite.name = "DrawLayer";

        rect = brushSprite.textureRect;
        brushHeight = (int)rect.height;
        brushWidth = (int)rect.width;

        if (!brushSprite.texture.isReadable)
        {
            var origTexPath = AssetDatabase.GetAssetPath(brushSprite.texture);
            var ti = (TextureImporter)AssetImporter.GetAtPath(origTexPath);
            ti.isReadable = true;
            ti.SaveAndReimport();
        }

        brushColorMap = brushSprite.texture.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
    }

    private void SetTexture()
    {
        drawTexture.SetPixels(drawColorMap[currentIndex]);
        drawTexture.Apply();
    }

    private void DrawBrush(Vector3 pos)
    {
        int xStart = Mathf.RoundToInt(pos.x * pixelsPerPoint - brushWidth * 0.5f);
        int xEnd = Mathf.RoundToInt(pos.x * pixelsPerPoint + brushWidth * 0.5f);
        int yStart = Mathf.RoundToInt(pos.y * pixelsPerPoint - brushHeight * 0.5f);
        int yEnd = Mathf.RoundToInt(pos.y * pixelsPerPoint + brushHeight * 0.5f);

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
                drawColorMap[currentIndex][j * textureWidth + i].a = alpha;
                drawAlphaMap[j * textureWidth + i] = alpha;
            }
        }
    }

    public void DrawPoint(Vector2 point)
    {
        var localpos = transform.InverseTransformPoint(point);
        localpos += Vector3.one * textureWidth * 0.5f / pixelsPerPoint;
        DrawBrush(localpos);
        SetTexture();
    }

    public void Roast()
    {
        if (currentIndex < layerSprites.Length - 1)
        {
            ++currentIndex;
            for (int i = 0; i < drawAlphaMap.Length; ++i)
            {
                drawColorMap[currentIndex][i].a = drawAlphaMap[i];
            }
            SetTexture();
        }
    }
}
