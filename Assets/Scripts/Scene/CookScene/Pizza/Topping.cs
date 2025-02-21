using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Topping : MonoBehaviour, IObjectPoolItem
{
    public IngredientTable.Data toppingData;
    private SpriteRenderer spriteRenderer;

    public IObjectPool<GameObject> ObjPool { get; set; }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetData(IngredientTable.Data toppingData)
    {
        this.toppingData = toppingData;
        spriteRenderer.sprite = toppingData.spriteDatas.toppingSprites[Random.Range(0, toppingData.spriteDatas.toppingSprites.Length)];
        
    }

    public void AddOrderOffset(int offset)
    {
        spriteRenderer.sortingOrder += offset;
    }

    public void Release()
    {
        toppingData = null;
        spriteRenderer.sprite = null;
        ObjPool.Release(gameObject);
    }
}
