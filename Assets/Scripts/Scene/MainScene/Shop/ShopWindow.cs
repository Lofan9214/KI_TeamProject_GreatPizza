using TMPro;
using UnityEngine;

public class ShopWindow : MonoBehaviour
{
    public Transform shopContent;
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

            var item = Instantiate(itemPrefab, shopContent);
            item.Init(data, ing.Value, day);
            item.OnBought.AddListener(() =>
            {
                budgetText.text = SaveLoadManager.Data.budget.ToString("F2");
            });
        }
    }
}
