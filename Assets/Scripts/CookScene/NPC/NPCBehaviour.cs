using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCBehaviour : MonoBehaviour
{
    private const string cheese = "cheese";

    public class JudgeData
    {
        public enum Judge
        {
            Fail,
            Normal,
            Good,
        }

        public Judge FinalJudge
        {
            get
            {
                return (Judge)Mathf.Min(
                    (int)dough,
                    (int)source,
                    (int)cutting,
                    (int)roasting,
                    ingredientsJudge.Count > 0 ? (int)Judge.Good : ingredientsJudge.Min(p => (int)p.judge));
            }
        }

        public Judge dough = Judge.Good;
        public Judge source = Judge.Good;
        public Judge cutting = Judge.Good;
        public Judge roasting = Judge.Good;

        public List<(string id, Judge judge)> ingredientsJudge = new List<(string id, Judge judge)>();
    }


    public static JudgeData GetJudgeData(RecipeTable.RecipeData recipe, Pizza.Data pizzaData)
    {
        int diffRoast = Mathf.Abs(recipe.roast - pizzaData.roastCount);
        int diffCutting = Mathf.Abs(recipe.cutting - pizzaData.cutData.Count);

        JudgeData judgeData = new JudgeData();
        JudgeData.Judge judge;
        IngredientTable.Data ingredientdata;
        int count;
        int ratio;

        // 도우 -> 소스 -> 토핑 -> 굽기 -> 커팅

        judgeData.dough = recipe.ingredientIds.Contains(pizzaData.doughID) ? JudgeData.Judge.Good : JudgeData.Judge.Normal;
        var cheesedata = DataTableManager.IngredientTable.Get(cheese);

        if (recipe.ingredientIds.Contains(cheese))
            ratio = Mathf.RoundToInt(pizzaData.sourceRatio * 100f);
        else
            ratio = 100 - Mathf.RoundToInt(pizzaData.sourceRatio * 100f);

        if (ratio <= cheesedata.fail)
            judgeData.source = JudgeData.Judge.Fail;
        else if (ratio < cheesedata.success)
            judgeData.source = JudgeData.Judge.Normal;
        else
            judgeData.source = JudgeData.Judge.Good;

        foreach (var ingredientId in recipe.ingredientIds)
        {
            count = pizzaData.toppingData.Where(p => string.Compare(p, ingredientId, true) == 0).Count();
            ingredientdata = DataTableManager.IngredientTable.Get(ingredientId);
            if (count <= ingredientdata.fail)
                judge = JudgeData.Judge.Fail;
            else if (count < ingredientdata.success)
                judge = JudgeData.Judge.Normal;
            else
                judge = JudgeData.Judge.Good;
            judgeData.ingredientsJudge.Add((ingredientId, judge));
        }

        if (pizzaData.roastCount >= 3 || diffRoast > 1)
            judgeData.roasting = JudgeData.Judge.Fail;
        else if (diffRoast == 1)
            judgeData.roasting = JudgeData.Judge.Normal;
        else
            judgeData.roasting = JudgeData.Judge.Good;

        if (diffCutting <= 2)
            judgeData.cutting = JudgeData.Judge.Good;
        else
            judgeData.cutting = JudgeData.Judge.Normal;

        return judgeData;
    }
}
