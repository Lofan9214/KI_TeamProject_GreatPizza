using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class NPCBehaviour : MonoBehaviour, IPizzaSlot
{
    private const string cheese = "cheese";

    public enum HintState
    {
        None,
        Hint1,
        Hint2,
    }

    public RecipeTable.RecipeData Recipe { get; private set; }

    public bool IsSettable => true;

    public bool IsEmpty => true;

    public Pizza CurrentPizza { get; private set; }

    public SpriteRenderer spriteRenderer;
    public ChatWindow chatWindow;
    public IngameGameManager gameManager;

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
                    ingredientsJudge.Count > 0 ? ingredientsJudge.Min(p => (int)p.judge) : (int)Judge.Success);
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

    public JudgeData GetJudgeData(RecipeTable.RecipeData recipe, Pizza.Data pizzaData)
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

    public void ClearPizza()
    {
    }

    public void SetPizza(Pizza go)
    {
        CurrentPizza = go;
        CurrentPizza.gameObject.SetActive(false);
        StartCoroutine(EndOrder());
    }

    private IEnumerator EndOrder()
    {
        var judgeData = GetJudgeData(Recipe, CurrentPizza.PizzaData);
        switch (judgeData.FinalJudge)
        {
            case JudgeData.Judge.Fail:
                chatWindow.NextTalk(ChatWindow.Talks.Fail);
                break;
            case JudgeData.Judge.Normal:
                chatWindow.NextTalk(ChatWindow.Talks.Normal);
                break;
            case JudgeData.Judge.Success:
                chatWindow.NextTalk(ChatWindow.Talks.Success);
                break;
        }
        yield return new WaitUntil(() => chatWindow.TalkingState == ChatWindow.State.Talkend);
        yield return new WaitForSeconds(0.5f);
        chatWindow.gameObject.SetActive(false);
        StartCoroutine(gameManager.Spawn());
    }
}
