using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Dough : MonoBehaviour
{
    public string IngredientId { get; private set; } = string.Empty;

    public Sprite[] layerSprites;

    private SpriteRenderer spriteRenderer;
    public UnityEvent<Sprite> OnSpriteChanged;
    private int currentIndex = 0;

    public Sprite CurrentSprite=>layerSprites[currentIndex];

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(string ingredientId)
    {
        IngredientId = ingredientId;
        layerSprites = DataTableManager.IngredientTable.Get(ingredientId).spriteDatas.toppingSprites;
        spriteRenderer.sprite = layerSprites[currentIndex];
        OnSpriteChanged?.Invoke(layerSprites[currentIndex]);
    }

    public void Roast()
    {
        if (currentIndex < layerSprites.Length - 1)
        {
            ++currentIndex;
            spriteRenderer.sprite = layerSprites[currentIndex];
            OnSpriteChanged?.Invoke(layerSprites[currentIndex]);
        }
    }
}
