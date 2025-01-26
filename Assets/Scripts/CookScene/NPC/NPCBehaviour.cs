using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class NPCBehaviour : MonoBehaviour
{
    public enum HintState
    {
        None,
        Hint1,
        Hint2,
    }

    private const string cheese = "cheese";

    public RecipeTable.RecipeData Recipe { get; private set; }

    private SpriteRenderer spriteRenderer;

    public class JudgeData
    {
        public enum Judge
        {
            Fail,
            Normal,
            Success,
        }

        public Judge FinalJudge
        {
            get
            {
                return (Judge)Mathf.Min((int)dough, (int)source, (int)cheese, (int)cutting, (int)roasting,
                    ingredientsJudge.Count > 0 ? (int)Judge.Success : ingredientsJudge.Min(p => (int)p.judge));
            }
        }

        public Judge dough = Judge.Success;
        public Judge source = Judge.Success;
        public Judge cheese = Judge.Success;
        public Judge cutting = Judge.Success;
        public Judge roasting = Judge.Success;
        public List<(string id, Judge judge)> ingredientsJudge = new List<(string id, Judge judge)>();
    }

    private static JudgeData.Judge JudgeValue(int value, int fail, int good)
    {
        if (value <= fail)
        {
            return JudgeData.Judge.Fail;
        }
        else if (value < good)
        {
            return JudgeData.Judge.Normal;
        }
        else
        {
            return JudgeData.Judge.Success;
        }
    }

    public void Order(RecipeTable.RecipeData recipe)
    {
        Recipe = recipe;
    }

    public void SetSprite(NPCTable.Data data)
    {
        spriteRenderer.sprite = data.Sprite;
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        bool cheeseExists = false;
        bool sourceExists = false;

        // 도우 -> 소스 -> 토핑 -> 굽기 -> 커팅

        judgeData.dough = recipe.dough == pizzaData.doughID ? JudgeData.Judge.Success : JudgeData.Judge.Normal;

        foreach (var ingredientId in recipe.ingredientIds)
        {
            var datum = DataTableManager.IngredientTable.Get(ingredientId);

            if (datum.type == 3)
            {
                count = pizzaData.toppingData.Where(p => string.Compare(p, ingredientId, true) == 0).Count();
                ingredientdata = DataTableManager.IngredientTable.Get(ingredientId);
                judge = JudgeValue(count, ingredientdata.fail, ingredientdata.success);
                judgeData.ingredientsJudge.Add((ingredientId, judge));
            }
            else if (datum.type == 2)
            {
                if (datum.ingredientID == cheese)
                {
                    cheeseExists = true;
                    ratio = Mathf.RoundToInt(pizzaData.cheeseRatio * 100f);
                    judgeData.cheese = JudgeValue(ratio, datum.fail, datum.success);
                }
                else
                {
                    sourceExists = true;
                    ratio = Mathf.RoundToInt(pizzaData.sourceRatio * 100f);
                    judgeData.source = JudgeValue(ratio, datum.fail, datum.success);
                }
            }
        }

        if (!cheeseExists)
        {
            ratio = 100 - Mathf.RoundToInt(pizzaData.cheeseRatio * 100f);
            var datum = DataTableManager.IngredientTable.Get(cheese);
            judgeData.cheese = JudgeValue(ratio, datum.fail, datum.success);
        }

        if (!sourceExists)
        {
            ratio = 100 - Mathf.RoundToInt(pizzaData.sourceRatio * 100f);
            var datum = DataTableManager.IngredientTable.Get(pizzaData.sourceId);
            judgeData.source = JudgeValue(ratio, datum.fail, datum.success);
        }

        if (pizzaData.roastCount >= 3 || diffRoast > 1)
            judgeData.roasting = JudgeData.Judge.Fail;
        else if (diffRoast == 1)
            judgeData.roasting = JudgeData.Judge.Normal;
        else
            judgeData.roasting = JudgeData.Judge.Success;

        if (diffCutting <= 2)
            judgeData.cutting = JudgeData.Judge.Success;
        else
            judgeData.cutting = JudgeData.Judge.Normal;

        return judgeData;
    }
}
