using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SpriteRotator : MonoBehaviour
{
    public enum Rotate
    {
        None,
        Right,
        Left,
    }

    public Sprite targetSprite;
    public Sprite resultSprite;
    public Rotate rotate;

    private Texture2D croppedTexture;

    private void OnEnable()
    {
        if (targetSprite != null
            && (resultSprite == null
            || targetSprite.textureRect.width != resultSprite.textureRect.height))
            RotateSprite();
    }

    public void RotateSprite()
    {
        if (targetSprite != null)
        {
            if (rotate == Rotate.None)
            {
                resultSprite = targetSprite;
                SetResultSprite();
            }
            else
            {

#if UNITY_EDITOR
                if (!targetSprite.texture.isReadable)
                {
                    var origTexPath = AssetDatabase.GetAssetPath(targetSprite.texture);
                    var ti = (TextureImporter)AssetImporter.GetAtPath(origTexPath);
                    ti.isReadable = true;
                    ti.SaveAndReimport();
                }
#endif
                var rect = targetSprite.textureRect;
                int rectwidth = (int)rect.width;
                int rectheight = (int)rect.height;
                int rotatedWidth = rotate == Rotate.None ? rectwidth : rectheight;
                int rotatedHeight = rotate == Rotate.None ? rectheight : rectwidth;
                croppedTexture = new Texture2D(rotatedWidth, rotatedHeight);
                var pixels = targetSprite.texture.GetPixels((int)rect.x, (int)rect.y, rectwidth, rectheight);

                Color[] rotatedpixels = new Color[pixels.Length];
                switch (rotate)
                {
                    case Rotate.Right:
                        for (int i = 0; i < rectheight; ++i)
                        {
                            for (int j = 0; j < rectwidth; ++j)
                            {
                                rotatedpixels[j * rotatedWidth + rectheight - i - 1] = pixels[i * rectwidth + j];
                            }
                        }
                        break;
                    case Rotate.Left:
                        for (int i = 0; i < rectheight; ++i)
                        {
                            for (int j = 0; j < rectwidth; ++j)
                            {

                                rotatedpixels[(rotatedHeight - j - 1) * rotatedWidth + i] = pixels[i * rectwidth + j];

                            }
                        }
                        break;
                }
                croppedTexture.SetPixels(rotatedpixels);
                croppedTexture.Apply();
                resultSprite = Sprite.Create(croppedTexture, new Rect(0f, 0f, croppedTexture.width, croppedTexture.height), Vector2.one * 0.5f, targetSprite.pixelsPerUnit);
                SetResultSprite();
            }
        }
    }

    public void SetResultSprite()
    {
        var image = GetComponent<Image>();
        if (image != null)
        {
            image.sprite = resultSprite;
            return;
        }
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = resultSprite;
        }
    }
}

