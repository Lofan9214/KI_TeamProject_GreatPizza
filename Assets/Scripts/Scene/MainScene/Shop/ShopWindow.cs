using TMPro;
using UnityEngine;

public class ShopWindow : MonoBehaviour
{
    public Transform ingredientContent;
    public Transform upgradeContent;
    public TextMeshProUGUI budgetText;
    public ShopItem itemPrefab;

    private void Start()
    {
        int day = SaveLoadManager.Data.days;
        budgetText.text = SaveLoadManager.Data.budget.ToString("F2");
        foreach (var ing in SaveLoadManager.Data.ingredients)
        {
            var data = DataTableManager.IngredientTable.Get(ing.Key);
            if (data.store_price < 0f)
            {
                continue;
            }

            var item = Instantiate(itemPrefab, ingredientContent);
            item.Init(data, ing.Value, day);
            //item.transform.parent = ingredientContent;
            item.OnBought.AddListener(() =>
            {
                budgetText.text = SaveLoadManager.Data.budget.ToString("F2");
            });
        }
        foreach (var upgrade in SaveLoadManager.Data.upgrades)
        {
            var data = DataTableManager.StoreTable.Get(upgrade.Key);
            var item = Instantiate(itemPrefab, upgradeContent);
            item.Init(data, upgrade.Value);
            //item.transform.parent = upgradeContent;
            item.OnBought.AddListener(() =>
            {
                budgetText.text = SaveLoadManager.Data.budget.ToString("F2");
            });
        }
    }

    public void AddBudget()
    {
        SaveLoadManager.Data.budget += 1000f;
        SaveLoadManager.Save();
        budgetText.text = SaveLoadManager.Data.budget.ToString("F2");
    }
}
