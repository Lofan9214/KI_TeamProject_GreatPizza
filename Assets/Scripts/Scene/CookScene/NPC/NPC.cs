using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class NPC : MonoBehaviour, IPizzaSlot
{
    public enum StoryState
    {
        Story,
        Random,
    }
    private readonly int disappearHash = Animator.StringToHash("Disappear");

    private const string cheese = "cheese";
    private WaitForSeconds wait = new WaitForSeconds(2f);
    private WaitUntil waitChatEnd;

    public Transform sprite;
    public ChatWindow chatWindow;
    public IngameGameManager gameManager;

    public TextMeshProUGUI tipText;
    public Transform tipTextPosition;

    public bool IsSettable => true;

    public bool IsEmpty => true;

    public Pizza CurrentPizza { get; private set; }
    public StoryState state { get; private set; }
    public RecipeTable.Data Recipe { get; private set; }

    private float payment;
    private GameObject prefab;
    private StoryTable.Data storyNPCData;
    private Animator animator;

    public bool Disappeared { get; private set; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        waitChatEnd = new WaitUntil(() => chatWindow.TalkingState == ChatWindow.State.Talkend);
        chatWindow.OnYes.AddListener(Pay);
        gameManager.timeManager.OnUnsatisfied.AddListener(() => StartCoroutine(CutOrder()));
        tipText.transform.position = Camera.main.WorldToScreenPoint(tipTextPosition.position);
        Disappeared = false;
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

    public void SetData(NPCTable.Data data)
    {
        storyNPCData = null;
        state = StoryState.Random;

        SetPrefab(data.Prefab);
    }

    public void SetData(StoryTable.Data data)
    {
        storyNPCData = data;
        state = StoryState.Story;

        SetPrefab(data.Prefab);
    }

    private void SetPrefab(GameObject iprefab)
    {
        if (prefab != null)
        {
            Destroy(prefab);
            prefab = null;
        }
        prefab = Instantiate(iprefab, sprite);
        animator.SetBool(disappearHash, false);
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
            if (datum != null)
            {
                judgeData.source = JudgeValue(ratio, datum.fail, datum.success);
            }
            else
            {
                datum = DataTableManager.IngredientTable.Get("tomato");
                judgeData.source = JudgeValue(ratio, datum.fail, datum.success);
            }
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
                gameManager.Refund(-payment);
                gameManager.uiManager.ShowTipMessage(-payment, 1f);
                StartCoroutine(TipShow(-payment));
                break;
            case JudgeData.Judge.Normal:
                chatWindow.NextTalk(ChatWindow.Talks.Normal);

                break;
            case JudgeData.Judge.Success:
                chatWindow.NextTalk(ChatWindow.Talks.Success);
                break;
        }

        if (state == StoryState.Random
            || storyNPCData.price < 0)
        {
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
                gameManager.AddTip(tip);
                tipText.text = tip.ToString("F2");
                tipText.gameObject.SetActive(true);
            }
        }
        else if (state == StoryState.Story && storyNPCData.price > 0)
        {
            gameManager.AddTip(storyNPCData.price);
            tipText.text = storyNPCData.price.ToString("F2");
            tipText.gameObject.SetActive(true);
        }

        yield return waitChatEnd;
        yield return wait;
        chatWindow.gameObject.SetActive(false);
        tipText.gameObject.SetActive(false);
        animator.SetBool(disappearHash, true);
    }

    public IEnumerator CutOrder()
    {
        gameManager.ChangePlace(InGamePlace.Hall);
        chatWindow.NextTalk(ChatWindow.Talks.Fail);
        gameManager.uiManager.ShowTipMessage(-payment, 1f);
        gameManager.Refund(-payment);
        gameManager.kitchen.packingTable.DestroyPizzaBox();
        yield return waitChatEnd;
        yield return new WaitForSeconds(0.5f);
        chatWindow.gameObject.SetActive(false);
        gameObject.SetActive(false);
        gameManager.StartCoroutine(gameManager.Spawn());
    }

    public void Pay()
    {
        payment = 0f;

        payment += DataTableManager.IngredientTable.Get(Recipe.dough).profit;

        foreach (var ing in Recipe.ingredientIds)
        {
            if (string.IsNullOrEmpty(ing) || ing == "0")
            {
                break;
            }
            payment += DataTableManager.IngredientTable.Get(ing).profit;
        }

        gameManager.Pay(payment);

        StartCoroutine(TipShow(payment));
    }

    private IEnumerator TipShow(float amount)
    {
        tipText.text = amount.ToString("F2");
        tipText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        tipText.gameObject.SetActive(false);
    }

    public float GoodTip()
    {
        float result = 0f;
        var data = DataTableManager.IngredientTable.Get(Recipe.dough);
        if (data != null)
            result += Random.Range(data.happy_min, data.happy_max);

        foreach (var ing in Recipe.ingredientIds)
        {
            if (string.IsNullOrEmpty(ing) || ing == "0")
            {
                break;
            }
            data = DataTableManager.IngredientTable.Get(ing);
            if (data != null)
                result += Random.Range(data.happy_min, data.happy_max);
        }
        return result;
    }
    public float NormalTip()
    {
        float result = 0f;
        var data = DataTableManager.IngredientTable.Get(Recipe.dough);
        if (data != null)
        {
            result += Random.Range(data.normal_min, data.normal_max);
        }

        foreach (var ing in Recipe.ingredientIds)
        {
            if (string.IsNullOrEmpty(ing) || ing == "0")
            {
                break;
            }
            data = DataTableManager.IngredientTable.Get(ing);
            if (data != null)
                result += Random.Range(data.normal_min, data.normal_max);
        }
        return result;
    }

    public void Appeared()
    {
        Disappeared = false;
    }

    public void SetDisappeard()
    {
        Disappeared = true;
        if (state == StoryState.Story)
        {
            gameManager.timeManager.SetWatch((storyNPCData.timeend / 100 - 12) * 4 + (storyNPCData.timeend % 100 / 15));
        }
        gameObject.SetActive(false);
        gameManager.StartCoroutine(gameManager.Spawn());
    }
}
