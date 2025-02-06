using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dough : MonoBehaviour
{
    public string IngredientId { get; private set; } = string.Empty;

    public Sprite[] layerSprites;

    private SpriteRenderer spriteRenderer;
    private int currentIndex = 0;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(string ingredientId)
    {
        IngredientId = ingredientId;
        layerSprites = DataTableManager.IngredientTable.Get(ingredientId).spriteDatas.toppingSprites;
        spriteRenderer.sprite = layerSprites[currentIndex];
    }

    public void Roast()
    {
        if (currentIndex < layerSprites.Length - 1)
        {
            ++currentIndex;
            spriteRenderer.sprite = layerSprites[currentIndex];
        }
    }
}
