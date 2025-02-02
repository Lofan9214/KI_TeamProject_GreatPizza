using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;

public class NPC : MonoBehaviour, IPizzaSlot
{
    private const string cheese = "cheese";
    private WaitForSeconds wait = new WaitForSeconds(2f);
    private WaitUntil waitChatEnd;

    public TextMeshProUGUI tipText;
    public Transform tipTextPosition;

    public RecipeTable.Data Recipe { get; private set; }

    public bool IsSettable => true;

    public bool IsEmpty => true;

    public Pizza CurrentPizza { get; private set; }

    public SpriteRenderer spriteRenderer;
    public ChatWindow chatWindow;
    public IngameGameManager gameManager;
    private float payment;

    private void Start()
    {
        waitChatEnd = new WaitUntil(() => chatWindow.TalkingState == ChatWindow.State.Talkend);
        chatWindow.OnYes.AddListener(Pay);
        gameManager.timeManager.OnUnsatisfied.AddListener(CutOrder);
    }

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

    public void Order(RecipeTable.Data recipe)
    {
        Recipe = recipe;
    }

    public void SetSprite(NPCTable.Data data)
    {
        spriteRenderer.sprite = data.Sprite;
    }

    public JudgeData GetJudgeData(RecipeTable.Data recipe, Pizza.Data pizzaData)
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

            if (datum == null)
            {
                continue;
            }

            switch (datum.type)
            {
                case IngredientTable.Type.Source:
                    sourceExists = true;
                    ratio = Mathf.RoundToInt(pizzaData.sourceRatio * 100f);
                    judgeData.source = JudgeValue(ratio, datum.fail, datum.success);
                    break;
                case IngredientTable.Type.Cheese:
                    cheeseExists = true;
                    ratio = Mathf.RoundToInt(pizzaData.cheeseRatio * 100f);
                    judgeData.cheese = JudgeValue(ratio, datum.fail, datum.success);
                    break;
                case IngredientTable.Type.Ingredient:
                    count = pizzaData.toppingData.Where(p => string.Compare(p, ingredientId, true) == 0).Count();
                    ingredientdata = DataTableManager.IngredientTable.Get(ingredientId);
                    judge = JudgeValue(count, ingredientdata.fail, ingredientdata.success);
                    judgeData.ingredientsJudge.Add((ingredientId, judge));
                    break;
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
        gameManager.timeManager.SetState(IngameTimeManager.State.OrderEnd);
        int satisfaction = gameManager.timeManager.Satisfaction;
        var judgeData = GetJudgeData(Recipe, CurrentPizza.PizzaData);
        float tip = 0f;

        switch (judgeData.FinalJudge)
        {
            case JudgeData.Judge.Fail:
                chatWindow.NextTalk(ChatWindow.Talks.Fail);
                gameManager.AddCurrency(-payment);
                break;
            case JudgeData.Judge.Normal:
                chatWindow.NextTalk(ChatWindow.Talks.Normal);

                break;
            case JudgeData.Judge.Success:
                chatWindow.NextTalk(ChatWindow.Talks.Success);
                break;
        }
        if (satisfaction > 50 && judgeData.FinalJudge == JudgeData.Judge.Success)
        {
            tip += GoodTip();
        }
        else if ((satisfaction > 50 && judgeData.FinalJudge == JudgeData.Judge.Normal)
                 || (satisfaction <= 50 && satisfaction > 20 && judgeData.FinalJudge == JudgeData.Judge.Success))
        {
            tip += NormalTip();
        }
        else if ((judgeData.FinalJudge == JudgeData.Judge.Success && satisfaction <= 20)
                || (judgeData.FinalJudge == JudgeData.Judge.Normal && satisfaction >= 50 && satisfaction > 20))
        {
            tip += Random.Range(0.2f, 0.7f);
        }
        else if (judgeData.FinalJudge == JudgeData.Judge.Normal && satisfaction <= 20)
        {
            tip += Random.Range(0f, 0.05f);
        }
        if (tip > 0f)
        {
            gameManager.AddCurrency(tip);
            tipText.transform.position = Camera.main.WorldToScreenPoint(tipTextPosition.position);
            tipText.text = tip.ToString("F2");
            tipText.gameObject.SetActive(true);
        }
        yield return waitChatEnd;
        yield return wait;
        chatWindow.gameObject.SetActive(false);
        tipText.gameObject.SetActive(false);
        gameObject.SetActive(false);
        gameManager.StartSpawn();
    }

    public void CutOrder()
    {
        chatWindow.gameObject.SetActive(false);
        gameObject.SetActive(false);
        gameManager.AddCurrency(-payment);
        gameManager.ChangePlace(InGamePlace.Hall);
        gameManager.StartSpawn();
    }

    public void Pay()
    {
        payment = 0f;

        payment += DataTableManager.IngredientTable.Get(Recipe.dough).profit;

        foreach (var ing in Recipe.ingredientIds)
        {
            if (string.IsNullOrEmpty(ing))
            {
                break;
            }
            payment += DataTableManager.IngredientTable.Get(ing).profit;
        }

        gameManager.AddCurrency(payment);
    }

    public float GoodTip()
    {
        float result = 0f;
        var data = DataTableManager.IngredientTable.Get(Recipe.dough);
        result += Random.Range(data.happy_min, data.happy_max);

        foreach (var ing in Recipe.ingredientIds)
        {
            if (string.IsNullOrEmpty(ing))
            {
                break;
            }
            data = DataTableManager.IngredientTable.Get(ing);
            result += Random.Range(data.happy_min, data.happy_max);
        }
        return result;
    }
    public float NormalTip()
    {
        float result = 0f;
        var data = DataTableManager.IngredientTable.Get(Recipe.dough);
        result += Random.Range(data.normal_min, data.normal_max);

        foreach (var ing in Recipe.ingredientIds)
        {
            if (string.IsNullOrEmpty(ing))
            {
                break;
            }
            data = DataTableManager.IngredientTable.Get(ing);
            result += Random.Range(data.normal_min, data.normal_max);
        }
        return result;
    }
}
