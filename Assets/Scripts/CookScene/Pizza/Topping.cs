using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Topping : MonoBehaviour
{
    public ToppingData toppingData;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetData(ToppingData data)
    {
        toppingData = data;
        spriteRenderer.sprite = toppingData.sprite;
    }

    public void AddOrderOffset(int offset)
    {
        spriteRenderer.sortingOrder += offset;
    }
}
