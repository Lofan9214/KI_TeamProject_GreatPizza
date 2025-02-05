using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Topping : MonoBehaviour
{
    private const string dataFormat = "SpriteDatas/{0}";
    public IngredientTable.Data toppingData;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetData(string dataId)
    {
        toppingData = DataTableManager.IngredientTable.Get(dataId);
        var spriteData = Resources.Load<IngredientSpriteData>(string.Format(dataFormat, dataId));
        spriteRenderer.sprite = spriteData.sprites[Random.Range(0,spriteData.sprites.Length)];
        transform.Rotate(0f, 0f, Random.Range(0, 360f));
    }

    public void AddOrderOffset(int offset)
    {
        spriteRenderer.sortingOrder += offset;
    }
}
