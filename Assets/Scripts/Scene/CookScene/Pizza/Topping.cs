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
        spriteRenderer.sprite = toppingData.Sprite;
    }

    public void AddOrderOffset(int offset)
    {
        spriteRenderer.sortingOrder += offset;
    }
}
