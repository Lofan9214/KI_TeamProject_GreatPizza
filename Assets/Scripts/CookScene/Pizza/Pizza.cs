using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

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
        public string doughID = "dough";
        public int roastCount = 0;
        public List<quaternion> cutData = new List<quaternion>();
        public List<string> toppingData = new List<string>();
        public float sourceRatio = 0f;
        public string sourceId = "tomato";
        public float cheeseRatio = 0f;
    }

    public State PizzaState { get; set; } = State.AddingTopping;

    public Data PizzaData { get; private set; } = new Data();

    public SpriteRenderer dough;
    public GameObject pizzaBoard;
    public DrawIngredient sourceLayer;
    public DrawIngredient cheeseLayer;
    public ToppingLayer toppingLayer;
    public SpriteRenderer roastLayer;
    public Cut cutLayer;
    public PizzaBox box;

    private Transform currentSlot;
    private Transform tempSlot;

    private IngameGameManager gameManager;

    private Vector3? lastDrawPos = null;
    private CircleCollider2D circleCollider;

    private void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        gameManager = GameObject.FindGameObjectWithTag("GameController")?.GetComponent<IngameGameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Slot"))
        {
            Debug.Log($"SlotFound: {collision.gameObject.name}");
            tempSlot = collision.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (tempSlot != null
            && tempSlot == collision.transform)
        {
            Debug.Log("SlotExit");
            tempSlot = null;
        }
    }

    public void OnDragEnd()
    {
        if (tempSlot != null
            && tempSlot != currentSlot)
        {
            var targetSocket = tempSlot.GetComponent<IPizzaSlot>();
            if (targetSocket != null
                && targetSocket.IsSettable
                && targetSocket.IsEmpty)
            {
                currentSlot?.GetComponent<IPizzaSlot>()?.ClearPizza();
                targetSocket.SetPizza(this);
                SetCurrentSlot(tempSlot);
                tempSlot = null;
                if (pizzaBoard.activeSelf)
                {
                    pizzaBoard.SetActive(false);
                }
                return;
            }
        }
        transform.position = currentSlot.position;

        PizzaData.sourceRatio = sourceLayer.Ratio;
        PizzaData.cheeseRatio = cheeseLayer.Ratio;

        lastDrawPos = null;
    }


    public void OnPressObject(Vector2 position)
    {
        if (PizzaState == State.AddingTopping
            && Vector2.Distance(position, transform.position) < circleCollider.radius)
        {
            switch (gameManager.PizzaCommand)
            {
                case PizzaCommand.Pepperoni:
                case PizzaCommand.Sausage:
                    string toppingId = gameManager.PizzaCommand.ToString().ToLower();
                    PizzaData.toppingData.Add(toppingId);
                    toppingLayer.AddTopping(position, toppingId);
                    break;
            }
        }
    }

    public void Move(Vector3 deltaPos)
    {
        transform.position += deltaPos;
    }

    public void OnDrag(Vector3 pos, Vector3 deltaPos)
    {
        switch (PizzaState)
        {
            case State.AddingTopping:
                if (lastDrawPos != null
                    && Vector2.Distance(pos, (Vector2)lastDrawPos) < 0.5f)
                {

                }

                switch (gameManager.PizzaCommand)
                {
                    case PizzaCommand.Source:
                        sourceLayer.DrawPoint(pos);
                        break;
                    case PizzaCommand.Cheese:
                        cheeseLayer.DrawPoint(pos);
                        break;
                }
                lastDrawPos = pos;
                break;
            case State.Movable:
                Move(deltaPos);
                break;
        }
    }

    public void OnDragFromBoard(Vector3 pos, Vector3 deltaPos)
    {
        if (Vector2.Distance(pos, transform.position) < circleCollider.radius)
        {
            OnDrag(pos, deltaPos);
            return;
        }
        if (lastDrawPos == null)
            Move(deltaPos);
    }

    public void Roast()
    {
        PizzaData.roastCount++;
        Color color = roastLayer.color;
        color.a += (1f - color.a) * 0.2f;
        roastLayer.color = color;
    }

    public void Cut(quaternion rotation)
    {
        PizzaData.cutData.Add(rotation);
        cutLayer.AddCut(rotation);
    }

    public void SetCurrentSlot(Transform slot)
    {
        currentSlot = slot;
    }

    public void SetDough(string doughId)
    {
        dough.sprite = DataTableManager.IngredientTable.Get(doughId).Sprite;
    }
}
