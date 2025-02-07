using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Topping : MonoBehaviour
{
    public IngredientTable.Data toppingData;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetData(string dataId)
    {
        toppingData = DataTableManager.IngredientTable.Get(dataId);
        spriteRenderer.sprite = toppingData.spriteDatas.toppingSprites[Random.Range(0, toppingData.spriteDatas.toppingSprites.Length)];
        transform.Rotate(0f, 0f, Random.Range(0, 360f));
    }

    public void AddOrderOffset(int offset)
    {
        spriteRenderer.sortingOrder += offset;
    }
}
