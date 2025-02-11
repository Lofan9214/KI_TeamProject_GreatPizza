using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class MainGameManager : MonoBehaviour
{
    public MainUIManager uiManager;
    public ShopTutorialManager tutorialPrefab;
    public ShopTutorialManager tutorialManager;

    public void Awake()
    {
        Application.targetFrameRate = 60;

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

        var storeData = DataTableManager.StoreTable.GetList();

        foreach (var upgrade in storeData)
        {
            if (!SaveLoadManager.Data.upgrades.ContainsKey(upgrade.storeID))
            {
                SaveLoadManager.Data.upgrades.Add(upgrade.storeID, false);
                added = true;
            }
        }

        if (added)
            SaveLoadManager.Save();

        if (SaveLoadManager.Data.days == 1
            && !SaveLoadManager.Data.ingredients["pepperoni"])
        {
            tutorialManager = Instantiate(tutorialPrefab, uiManager.transform);
            tutorialManager.SetState(ShopTutorialManager.State.StoreSelect);
        }
    }
}
