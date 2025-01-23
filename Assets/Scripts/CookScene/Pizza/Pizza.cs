using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class PizzaData
{
    public string doughID = string.Empty;
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

    public GameObject pizzaBoard;
    public DrawIngredient sourceLayer;
    public DrawIngredient cheeseLayer;
    public ToppingLayer toppingLayer;
    public SpriteRenderer bakeLayer;
    public Cut cutLayer;

    private Transform currentSocket;
    private Transform tempSocket;

    private GameManager gameManager;

    private Vector3 lastDrawPos;

    private void Start()
    {
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
        if (PizzaState == State.AddingTopping)
        {
            if (gameManager.PizzaCommand == PizzaCommand.Pepperoni)
            {
                PizzaData.toppingData.Add(gameManager.PizzaCommand.ToString());
                toppingLayer.AddTopping(position);
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
                switch (gameManager.PizzaCommand)
                {
                    case PizzaCommand.Source:
                        sourceLayer.DrawPoint(pos);
                        break;
                    case PizzaCommand.Cheese:
                        cheeseLayer.DrawPoint(pos);
                        break;
                }
                break;
            case State.Movable:
                Move(deltaPos);
                break;
        }
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
topping:{PizzaData.toppingData.Where(p => p == "Pepperoni").Count()}");
        }
    }
}
