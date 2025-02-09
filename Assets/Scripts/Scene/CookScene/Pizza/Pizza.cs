using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class Pizza : MonoBehaviour, IClickable, IDragable
{
    public enum State
    {
        AddingTopping,
        Movable,
        Immovable,
    }

    public class Data
    {
        public string doughID = string.Empty;
        public int roastCount = 0;
        public List<quaternion> cutData = new List<quaternion>();
        public List<string> toppingData = new List<string>();
        public float sourceRatio = 0f;
        public string sourceId = string.Empty;
        public float cheeseRatio = 0f;
    }

    public State CurrentState { get; set; } = State.AddingTopping;

    public Data PizzaData { get; private set; } = new Data();

    public Dough dough;
    public GameObject pizzaBoard;
    public DrawIngredient sourceLayer;
    public DrawIngredient cheeseLayer;
    public ToppingLayer toppingLayer;
    public SpriteRenderer roastLayer;
    public GameObject ingredientGuide;
    public GameObject cutGuide;
    public Cut cutLayer;
    public LayerMask slotMask;

    private Transform currentSlot;

    private IngameGameManager gameManager;
    private SpriteMask spriteMask;

    private Vector3? lastDrawPos = null;
    private Vector3? lastMovePos = null;
    private CircleCollider2D circleCollider;

    public float sourceMax = 1.5f;
    private float cheeseCurrent = 0f;
    private float sourceCurrent = 0f;
    private bool addingTopping = false;

    private void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        gameManager = GameObject.FindGameObjectWithTag("GameController")?.GetComponent<IngameGameManager>();
        spriteMask = GetComponent<SpriteMask>();
        dough.OnSpriteChanged.AddListener(p => spriteMask.sprite = p);
    }

    public void OnDragEnd(Vector3 pos, Vector3 deltaPos)
    {
        if (CurrentState == State.Immovable)
        {
            return;
        }
        DragEndSlot(pos, deltaPos);
    }

    public void DragEndSlot(Vector3 pos, Vector3 deltaPos)
    {
        if (CurrentState == State.AddingTopping
            && addingTopping
            && (gameManager.IngredientType == IngredientTable.Type.Source
                || gameManager.IngredientType == IngredientTable.Type.Cheese))
        {
            addingTopping = false;
            switch (gameManager.PizzaCommand)
            {
                case "tomato":
                    DrawSource(pos);
                    PizzaData.sourceRatio = sourceLayer.Ratio;
                    break;
                case "cheese":
                    DrawCheese(pos);
                    PizzaData.cheeseRatio = cheeseLayer.Ratio;
                    break;
            }
            return;
        }

        var slots = Physics2D.OverlapCircleAll(transform.position, circleCollider.radius, slotMask);

        if (slots.Length > 0)
        {
            var closest = slots.OrderBy(p => Vector3.Distance(p.transform.position, transform.position)).First();

            if (CurrentState == State.AddingTopping)
            {

                var slot = closest.GetComponent<IPizzaSlot>();
                if (slot != null
                    && (slot is OvenEnter || slot is TrashBin)
                    && slot.IsSettable
                    && slot.IsEmpty)
                {
                    currentSlot?.GetComponent<IPizzaSlot>()?.ClearPizza();
                    slot.SetPizza(this);
                    SetCurrentSlot(closest.transform);
                    if (pizzaBoard.activeSelf)
                    {
                        pizzaBoard.SetActive(false);
                    }
                    return;
                }
            }
            else
            {
                var slot = closest.GetComponent<IPizzaSlot>();
                if (slot != null
                    && slot.IsSettable
                    && slot.IsEmpty)
                {
                    currentSlot?.GetComponent<IPizzaSlot>()?.ClearPizza();
                    slot.SetPizza(this);
                    SetCurrentSlot(closest.transform);
                    if (pizzaBoard.activeSelf)
                    {
                        pizzaBoard.SetActive(false);
                    }
                    if (slot is CuttingSlot)
                    {
                        cutGuide.SetActive(true);
                    }
                    return;
                }
            }
        }
        if (currentSlot.GetComponent<CuttingSlot>())
        {
            cutGuide.SetActive(true);
        }
        if (currentSlot.gameObject.name.Contains("IngredientSlot")
            && gameManager.IngredientType == IngredientTable.Type.Ingredient)
        {
            ingredientGuide.SetActive(true);
        }
        transform.position = currentSlot.position;
    }

    public void OnPressObject(Vector2 position)
    {
        lastDrawPos = null;
        lastMovePos = null;
        addingTopping = false;
        if (CurrentState == State.AddingTopping
            && Vector2.Distance(position, transform.position) < circleCollider.radius)
        {
            if (gameManager.IngredientType == IngredientTable.Type.Source)
            {
                if (string.IsNullOrEmpty(sourceLayer.IngredientId)
                    && string.IsNullOrEmpty(PizzaData.sourceId))
                {
                    sourceLayer.Init(gameManager.PizzaCommand);
                    PizzaData.sourceId = gameManager.PizzaCommand;
                }
                if (gameManager.PizzaCommand == PizzaData.sourceId)
                {
                    DrawSource(position);
                    addingTopping = true;
                }
                lastDrawPos = position;
            }
            else if (gameManager.IngredientType == IngredientTable.Type.Cheese)
            {
                if (string.IsNullOrEmpty(cheeseLayer.IngredientId))
                {
                    cheeseLayer.Init(gameManager.PizzaCommand);
                }
                if (gameManager.PizzaCommand == cheeseLayer.IngredientId)
                {
                    DrawCheese(position);
                    addingTopping = true;
                }
                lastDrawPos = position;
            }
            else if (gameManager.IngredientType == IngredientTable.Type.Ingredient)
            {
                gameManager.IngredientPay(-DataTableManager.IngredientTable.Get(gameManager.PizzaCommand).price);
                PizzaData.toppingData.Add(gameManager.PizzaCommand);
                toppingLayer.AddTopping(position, gameManager.PizzaCommand);
            }
        }
    }

    public void Move(Vector3 Pos)
    {
        ingredientGuide.SetActive(false);
        cutGuide.SetActive(false);
        if (lastMovePos == null)
        {
            lastMovePos = Pos;
        }
        else
        {
            gameManager.ScrollScreen();
            transform.position += Pos - lastMovePos.Value;
            lastMovePos = Pos;
        }
    }

    public void OnDrag(Vector3 position, Vector3 deltaPos)
    {
        switch (CurrentState)
        {
            case State.AddingTopping:
                if (lastDrawPos == null)
                {
                    break;
                }
                if (lastDrawPos != null
                   && (Vector2.Distance(position, (Vector2)lastDrawPos) < 0.25f
                      || Vector2.Distance(position, transform.position) > circleCollider.radius))
                {
                    break;
                }

                switch (gameManager.IngredientType)
                {
                    case IngredientTable.Type.Source:
                        DrawSource(position);
                        break;
                    case IngredientTable.Type.Cheese:
                        DrawCheese(position);
                        break;
                }
                lastDrawPos = position;
                break;
            case State.Movable:
                Move(position);
                break;
        }
    }

    public void OnDragFromBoard(Vector3 position, Vector3 deltaPos)
    {
        if (Vector2.Distance(position, transform.position) < circleCollider.radius)
        {
            OnDrag(position, deltaPos);
            return;
        }
        if (lastDrawPos == null)
        {
            Move(position);
        }
    }

    public void Roast()
    {
        PizzaData.roastCount++;

        dough.Roast();
        sourceLayer.Roast();
        cheeseLayer.Roast();

        if (PizzaData.roastCount > 2)
        {
            Color color = roastLayer.color;
            color.a += (1f - color.a) * 0.2f;
            roastLayer.color = color;
        }
    }

    public void Cut(quaternion rotation)
    {
        PizzaData.cutData.Add(rotation);
        cutLayer.AddCut(rotation);
        if (PizzaData.cutData.Count > 2)
        {
            cutGuide.gameObject.SetActive(false);
        }
    }

    public void SetCurrentSlot(Transform slot)
    {
        currentSlot = slot;
    }

    public void SetDough(string doughId)
    {
        PizzaData.doughID = doughId;
        dough.Init(doughId);
    }

    public void DrawSource(Vector2 position)
    {
        sourceLayer.DrawPoint(position);
        if (sourceCurrent < 1.5f)
        {
            sourceCurrent += 0.01f;
            gameManager.IngredientPay(-0.01f);
        }
    }

    public void DrawCheese(Vector2 position)
    {
        cheeseLayer.DrawPoint(position);
        if (cheeseCurrent < 1.5f)
        {
            cheeseCurrent += 0.01f;
            gameManager.IngredientPay(-0.01f);
        }
    }
}
