using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using static TutorialManager;
using Random = UnityEngine.Random;

public class Pizza : MonoBehaviour, IClickable, IDragable
{
    public enum State
    {
        AddingTopping,
        Roasting,
        Roasted,
        Cut,
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

    private Data pizzaData = new Data();

    public Data PizzaData
    {
        get
        {
            pizzaData.sourceRatio = sourceLayer.Ratio;
            pizzaData.cheeseRatio = cheeseLayer.Ratio;
            return pizzaData;
        }
        private set
        {
            pizzaData = value;
        }
    }

    public Dough dough;
    public Board pizzaBoard;
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
    public CircleCollider2D CircleCollider { get; private set; }

    public float sourceMax = 1.5f;
    private float cheeseCurrent = 0f;
    private float sourceCurrent = 0f;
    private bool addingTopping = false;

    private IngredientTable.Data sourceData;
    private IngredientTable.Data cheeseData;
    private AudioSource audioSource;

    public Transform[] ingredientGuidePosition;
    private int autoIngredient;

    public bool homeComing { get; private set; } = false;

    private static List<Vector2> sourcePoints = new List<Vector2>();

    public bool Movable { get; set; } = true;
    private bool moving = false;

    private void Start()
    {
        CircleCollider = GetComponent<CircleCollider2D>();
        gameManager = GameObject.FindGameObjectWithTag("GameController")?.GetComponent<IngameGameManager>();
        spriteMask = GetComponent<SpriteMask>();
        audioSource = GetComponent<AudioSource>();
        dough.OnSpriteChanged.AddListener(p => spriteMask.sprite = p);
        autoIngredient = 0;

        if (sourcePoints.Count == 0)
        {
            sourcePoints.Add(Vector2.zero);
            float unit = 0.26f;
            float radius = unit;
            float angle = 0f;
            while (sourcePoints.Count < 120)
            {
                angle += unit / radius;
                if (angle > 2 * Mathf.PI)
                {
                    radius += unit;
                    angle = 0f;
                }
                sourcePoints.Add(new Vector2(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle)));
            }
        }
    }

    public void OnDragEnd(Vector3 pos, Vector3 deltaPos)
    {
        DragEndSlot(pos, deltaPos);
    }

    public void DragEndSlot(Vector3 pos, Vector3 deltaPos)
    {
        if (!Movable || homeComing)
        {
            return;
        }
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
                    break;
                case "cheese":
                    DrawCheese(pos);
                    break;
            }
            return;
        }

        var slots = Physics2D.OverlapCircleAll(transform.position, CircleCollider.radius, slotMask);

        if (slots.Length > 0)
        {
            var closest = slots.OrderBy(p => Vector2.SqrMagnitude(p.transform.position - transform.position)).First();
            var slot = closest.GetComponent<IPizzaSlot>();
            if (slot != null
                    && slot.IsSettable
                    && slot.IsEmpty)
            {
                if (CurrentState == State.AddingTopping
                     && (slot is OvenEnter || slot is TrashBin))
                {
                    SetPizza(slot, closest.transform);
                    return;
                }
                else if (CurrentState != State.Cut || slot is not OvenEnter)
                {
                    SetPizza(slot, closest.transform);
                    if (slot is CuttingSlot)
                    {
                        CurrentState = State.Cut;
                        if (gameManager.npc.Recipe.cutting > 0)
                            cutGuide.SetActive(true);
                    }
                    return;
                }
            }
        }
        if (currentSlot.GetComponent<CuttingSlot>() && gameManager.npc.Recipe.cutting > 0)
        {
            cutGuide.SetActive(true);
        }
        if (currentSlot.gameObject.name.Contains("IngredientSlot")
            && gameManager.IngredientType == IngredientTable.Type.Ingredient)
        {
            ingredientGuide.SetActive(true);
        }
        StartCoroutine(GoBack());
    }

    public void OnPressObject(Vector2 position)
    {
        if (homeComing)
        {
            gameManager.pointerManager.ClearDrag();
            return;
        }
        lastDrawPos = null;
        lastMovePos = null;
        moving = false;
        addingTopping = false;
        if (CurrentState == State.AddingTopping
            && Vector2.Distance(position, transform.position) < CircleCollider.radius)
        {
            if (gameManager.IngredientType == IngredientTable.Type.Source)
            {
                if (string.IsNullOrEmpty(sourceLayer.IngredientId)
                    && string.IsNullOrEmpty(PizzaData.sourceId))
                {
                    sourceData = DataTableManager.IngredientTable.Get(gameManager.PizzaCommand);
                    sourceLayer.Init(sourceData);
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
                    cheeseData = DataTableManager.IngredientTable.Get(gameManager.PizzaCommand);
                    cheeseLayer.Init(cheeseData);
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
                AddTopping(position, gameManager.PizzaCommand);
            }
            else if (gameManager.IngredientType != IngredientTable.Type.Ingredient
                && lastDrawPos == null)
            {
                moving = true;
                lastMovePos = position;
            }
        }
        else
        {
            moving = true;
            lastMovePos = position;
        }
    }

    private void SetPizza(IPizzaSlot slot, Transform closest)
    {
        currentSlot?.GetComponent<IPizzaSlot>()?.ClearPizza();
        slot.SetPizza(this);
        SetCurrentSlot(closest);
        if (pizzaBoard.gameObject.activeSelf)
        {
            pizzaBoard.gameObject.SetActive(false);
        }
    }

    public void AddTopping(Vector2 position, string Id)
    {
        var data = DataTableManager.IngredientTable.Get(Id);
        gameManager.IngredientPay(-data.price);
        PizzaData.toppingData.Add(Id);
        audioSource.PlayOneShot(data.spriteDatas.soundEffect);
        toppingLayer.AddTopping(position, data);
    }

    public void Move(Vector3 Pos, Vector3 deltaPos)
    {
        if (!Movable || autoIngredient > 0)
        {
            gameManager.pointerManager.ClearDrag();
            return;
        }

        ingredientGuide.SetActive(false);
        cutGuide.SetActive(false);

        if (gameManager.state == IngameGameManager.State.Random
            || (gameManager.tutorialManager.MaskLock && IsValidMove(deltaPos)))
        {
            gameManager.ScrollScreen();
            //transform.position += Pos - lastMovePos.Value;
            //lastMovePos = Pos;
            transform.position += deltaPos;
            lastMovePos = transform.position;
        }
    }

    public void OnDrag(Vector3 position, Vector3 deltaPos)
    {
        if (homeComing)
        {
            gameManager.pointerManager.ClearDrag();
            return;
        }
        switch (CurrentState)
        {
            case State.AddingTopping:
                if (moving)
                {
                    Move(position, deltaPos);
                    break;
                }
                if (lastDrawPos == null)
                {
                    break;
                }
                else if (Vector2.SqrMagnitude((Vector2)position - (Vector2)lastDrawPos) < 0.125f * 0.125f
                      || Vector2.SqrMagnitude((Vector2)position - (Vector2)transform.position) > CircleCollider.radius * CircleCollider.radius)
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
            case State.Roasted:
            case State.Cut:
                Move(position, deltaPos);
                break;
        }
    }

    public void OnDragFromBoard(Vector3 position, Vector3 deltaPos)
    {
        if (homeComing)
        {
            gameManager.pointerManager.ClearDrag();
            return;
        }
        if (Vector2.SqrMagnitude(position - transform.position) < CircleCollider.radius * CircleCollider.radius)
        {
            OnDrag(position, deltaPos);
            return;
        }
        if (moving
            && (gameManager.state == IngameGameManager.State.Random
               || (gameManager.tutorialManager.MaskLock && IsValidMove(deltaPos))))
        {
            Move(position, deltaPos);
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
        if (sourceCurrent < sourceMax)
        {
            sourceCurrent += 0.005f;
            audioSource.PlayOneShot(sourceData.spriteDatas.soundEffect);
            gameManager.IngredientPay(-0.005f);
        }
    }

    public void DrawCheese(Vector2 position)
    {
        cheeseLayer.DrawPoint(position);
        if (cheeseCurrent < sourceMax)
        {
            cheeseCurrent += 0.005f;
            audioSource.PlayOneShot(cheeseData.spriteDatas.soundEffect);
            gameManager.IngredientPay(-0.005f);
        }
    }

    public IEnumerator AutoIngredient(string ingredientId)
    {
        ++autoIngredient;
        int halfLength = ingredientGuidePosition.Length / 2;
        WaitForSeconds wait = new WaitForSeconds(1f / halfLength);
        for (int i = 0; i < halfLength; ++i)
        {
            yield return wait;
            AddTopping(ingredientGuidePosition[i].position, ingredientId);
            AddTopping(ingredientGuidePosition[i + halfLength].position, ingredientId);
        }
        --autoIngredient;
    }

    public IEnumerator AutoDraw(string command)
    {
        ++autoIngredient;
        UnityAction<Vector2> draw = null;
        float timer = 0f;
        int count = 0;
        switch (command)
        {
            case "tomato":
                sourceData = DataTableManager.IngredientTable.Get(command);
                sourceLayer.Init(sourceData);
                PizzaData.sourceId = command;
                draw = DrawSource;
                break;
            case "cheese":
                cheeseData = DataTableManager.IngredientTable.Get(command);
                cheeseLayer.Init(cheeseData);
                draw = DrawCheese;
                break;
        }

        var list = sourcePoints.ToList();

        for (int i = 0; i < list.Count - 1; ++i)
        {
            int j = Random.Range(i, list.Count);
            Vector2 temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }

        while (timer < 1f)
        {
            timer += Time.deltaTime;
            int timerCount = (int)(list.Count * timer);
            if (timerCount > list.Count)
                timerCount = list.Count;
            while (count < timerCount)
            {
                draw.Invoke((Vector2)transform.position + list[count]);
                ++count;
            }

            yield return null;
        }
        --autoIngredient;
    }

    public IEnumerator GoBack()
    {
        homeComing = true;
        while (lastDrawPos == null && Vector3.SqrMagnitude(transform.position - currentSlot.position) > 0.05f)
        {
            transform.position = Vector3.Lerp(transform.position, currentSlot.position, Time.deltaTime * 15f);
            yield return null;
        }
        transform.position = currentSlot.position;
        homeComing = false;
    }

    private bool IsValidMove(Vector3 deltaPos)
    {
        var pizzaBoardBounds = gameManager.tutorialManager.tutorialState == TutorialState.OvenEnter ? pizzaBoard.boxCollider.bounds : dough.spriteRenderer.bounds;
        Bounds bounds = gameManager.tutorialManager.LockBounds;
        if (pizzaBoardBounds.min.x + deltaPos.x < bounds.min.x
            || pizzaBoardBounds.max.x + deltaPos.x > bounds.max.x
            || pizzaBoardBounds.min.y + deltaPos.y < bounds.min.y
            || pizzaBoardBounds.max.y + deltaPos.y > bounds.max.y)
        {
            return false;
        }
        return true;
    }
}
