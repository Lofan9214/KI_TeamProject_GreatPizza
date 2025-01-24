using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class PizzaData
{
    public string doughID = "dough";
    public int bakeCount = 0;
    public List<quaternion> cutData = new List<quaternion>();
    public List<string> toppingData = new List<string>();
    public float sourceRatio = 0f;
    public float cheeseRatio = 0f;
}

public class Pizza : MonoBehaviour, IClickable, IDragable
{
    public enum State
    {
        AddingTopping,
        Movable,
        Immovable,
    }

    public State PizzaState { get; set; } = State.AddingTopping;

    public PizzaData PizzaData { get; private set; } = new PizzaData();

    public SpriteRenderer dough;
    public GameObject pizzaBoard;
    public DrawIngredient sourceLayer;
    public DrawIngredient cheeseLayer;
    public ToppingLayer toppingLayer;
    public SpriteRenderer bakeLayer;
    public Cut cutLayer;

    private Transform currentSocket;
    private Transform tempSocket;

    private GameManager gameManager;

    private Vector3? lastDrawPos = null;
    private CircleCollider2D circleCollider;

    private void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        gameManager = GameObject.FindGameObjectWithTag("GameController")?.GetComponent<GameManager>();
    }

    public void SetSocket(Transform go)
    {
        currentSocket = go;
    }

    public void OnDragEnd()
    {
        if (tempSocket != null
            && tempSocket != currentSocket)
        {
            var targetSocket = tempSocket.GetComponent<IPizzaSocket>();
            if (targetSocket != null
                && targetSocket.IsSettable
                && targetSocket.IsEmpty)
            {
                currentSocket?.GetComponent<IPizzaSocket>()?.ClearPizza();
                targetSocket.SetPizza(this);
                currentSocket = tempSocket;
                tempSocket = null;
                if (pizzaBoard.activeSelf)
                {
                    pizzaBoard.SetActive(false);
                }
                return;
            }
        }
        transform.position = currentSocket.position;

        PizzaData.sourceRatio = sourceLayer.Ratio();
        PizzaData.cheeseRatio = cheeseLayer.Ratio();

        lastDrawPos = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Socket"))
        {
            Debug.Log($"SocketFound{collision.gameObject.name}");
            tempSocket = collision.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (tempSocket != null
            && tempSocket == collision.transform)
        {
            Debug.Log("SocketExit");
            tempSocket = null;
        }
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
        if(lastDrawPos==null)
        Move(deltaPos);
    }

    public void Bake()
    {
        PizzaData.bakeCount++;
        Color color = bakeLayer.color;
        color.a += (1f - color.a) * 0.2f;
        bakeLayer.color = color;
    }

    public void Cut(quaternion rotation)
    {
        PizzaData.cutData.Add(rotation);
        cutLayer.AddCut(rotation);
    }

    public void SetCurrentSocket(Transform sock)
    {
        currentSocket = sock;
    }

    public void Update()
    {
        if (MultiTouchManager.Instance.DoubleTap)
        {
            Debug.Log(
@$"BakeCount: {PizzaData.bakeCount}
CutCount: {PizzaData.cutData.Count}
sourceLayer:{sourceLayer.Ratio() * 100f:F0}
cheeseLayer:{cheeseLayer.Ratio() * 100f:F0}
topping:{PizzaData.toppingData.Where(p => p == "pepperoni").Count()}");
        }
    }
}
