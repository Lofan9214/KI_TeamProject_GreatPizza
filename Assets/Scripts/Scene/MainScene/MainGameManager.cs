using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameManager : MonoBehaviour
{
    public MainUIManager uiManager;
    public ShopTutorialManager tutorialPrefab;
    public ShopTutorialManager tutorialManager;

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

        if(SaveLoadManager.Data.days == 1
            && !SaveLoadManager.Data.ingredients["pepperoni"])
        {
            tutorialManager = Instantiate(tutorialPrefab, uiManager.transform);
            tutorialManager.SetState(ShopTutorialManager.State.StoreSelect);
        }
    }
}
