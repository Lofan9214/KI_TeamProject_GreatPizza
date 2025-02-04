using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameManager : MonoBehaviour
{
    public void Awake()
    {
        var ingredientData = DataTableManager.IngredientTable.GetList();

        bool added = false;

        foreach (var ing in ingredientData)
        {
            if (!SaveLoadManager.Data.ingredients.ContainsKey(ing.ingredientID))
            {
                SaveLoadManager.Data.ingredients.Add(ing.ingredientID, ing.store_price < 0f);
                added = true;
            }
        }

        if (added)
            SaveLoadManager.Save();
    }
}
