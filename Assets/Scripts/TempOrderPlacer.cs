using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class TempOrderPlacer : MonoBehaviour
{
    private List<RecipeTable.RecipeData> recipeDatas;

    public RecipeTable.RecipeData RecipeData { get; private set; }

    public TextMeshProUGUI recipeId;
    public TextMeshProUGUI roast;
    public TextMeshProUGUI cutting;
    public TextMeshProUGUI ingredients;

    public TextMeshProUGUI totalJudge;
    public TextMeshProUGUI roastJudge;
    public TextMeshProUGUI cuttingJudge;
    public TextMeshProUGUI ingredientJudge;

    public PizzaSocket sellingCounter;

    private void Start()
    {
        recipeDatas = DataTableManager.RecipeTable.GetList();
    }

    public void RandomPlace()
    {
        Func<RecipeTable.RecipeData, bool> filter =
            p =>
            {
                bool contains = true;
                foreach(var id in p.ingredientIds)
                {
                    if (!PlayerData.unlocks.Contains(id))
                        contains = false;
                }
                return contains;
            };

        List<RecipeTable.RecipeData> filtered = DataTableManager.RecipeTable.GetList().Where(filter).ToList();

        int randomindex = Random.Range(0, filtered.Count);
        RecipeData = filtered[randomindex];

        recipeId.text = $"RecipeID: {RecipeData.recipeID}";
        roast.text = $"RoastCount: {RecipeData.roast}";
        cutting.text = $"CuttingCount: {RecipeData.cutting}";
        ingredients.text = $"Ingredients {string.Join(',', RecipeData.ingredientIds)}";
    }

    public void Judge()
    {
        if (!sellingCounter.IsEmpty
            && RecipeData != null)
        {
            var pizzaData = sellingCounter.CurrentPizza.PizzaData;
            int roastJudges = RecipeData.roast == pizzaData.bakeCount ? 2 : 0;
            int cutJudges = RecipeData.cutting == pizzaData.cutData.Count ? 2 : 0;

            List<int> judges = new List<int>();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < RecipeData.ingredientIds.Length; ++i)
            {
                switch (RecipeData.ingredientIds[i])
                {
                    case "tomato":
                        judges.Add(pizzaData.sourceRatio > 0.7f ? 2 : 0);
                        break;
                    case "cheese":
                        judges.Add(pizzaData.cheeseRatio > 0.7f ? 2 : 0);
                        break;
                    case "dough":
                        judges.Add(pizzaData.doughID == RecipeData.ingredientIds[i] ? 2 : 0);
                        break;
                    default:
                        int successTH = DataTableManager.IngredientTable.Get(RecipeData.ingredientIds[i]).success;
                        int cnt = pizzaData.toppingData.Where(p => p == RecipeData.ingredientIds[i]).Count();
                        judges.Add(cnt >= successTH ? 2 : 0);
                        sb.Append($"{RecipeData.ingredientIds[i]}:{cnt}_{(cnt >= successTH ?"OK":"NG")}, ");
                        break;
                }
            }
            int totaljudge = Mathf.Min(cutJudges, roastJudges);
            if (judges.Count > 0)
            {
                totaljudge = Mathf.Min(totaljudge, judges.Min());
            }
            roastJudge.text = $"Roast: {RecipeData.roast} {(roastJudges == 2 ? "OK" : "NG")}";
            cuttingJudge.text = $"Cutting: {RecipeData.cutting} {(cutJudges == 2 ? "OK" : "NG")}";
            ingredientJudge.text = $"Ingredient: {sb} Total - {(judges.Min() == 2 ? "OK" : "NG")}";
            totalJudge.text = $"TotalJudge: {(totaljudge == 2 ? "OK" : "NG")}";
        }
    }
}
