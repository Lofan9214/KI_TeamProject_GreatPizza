using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopWindow : MonoBehaviour
{
    public Transform shopContent;
    public ShopItem itemPrefab;

    private void Start()
    {
        int day = SaveLoadManager.Data.days;
        foreach (var ing in SaveLoadManager.Data.ingredients)
        {
            var data = DataTableManager.IngredientTable.Get(ing.Key);
            if (data.shopprice < 0f)
            {
                continue;
            }

            var item = Instantiate(itemPrefab, shopContent);
            item.Init(data, ing.Value, day);
        }
    }
}
