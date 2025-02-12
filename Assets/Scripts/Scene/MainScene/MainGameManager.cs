using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainGameManager : MonoBehaviour
{
    public MainUIManager uiManager;
    public ShopTutorialManager tutorialPrefab;
    public ShopTutorialManager tutorialManager;

    public void Awake()
    {
        Application.targetFrameRate = 60;


        bool added = false;
        var ingredientData = DataTableManager.IngredientTable.GetList();

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

    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }

    public void NewGame()
    {
        SaveLoadManager.Data = new SaveDataV2();

        var ingredientData = DataTableManager.IngredientTable.GetList();
        foreach (var ing in ingredientData)
        {
            if (!SaveLoadManager.Data.ingredients.ContainsKey(ing.ingredientID))
            {
                SaveLoadManager.Data.ingredients.Add(ing.ingredientID, ing.store_price < 0f);
            }
        }

        var storeData = DataTableManager.StoreTable.GetList();
        foreach (var upgrade in storeData)
        {
            if (!SaveLoadManager.Data.upgrades.ContainsKey(upgrade.storeID))
            {
                SaveLoadManager.Data.upgrades.Add(upgrade.storeID, false);
            }
        }

        SaveLoadManager.Save();

        StartGame();
    }
}
