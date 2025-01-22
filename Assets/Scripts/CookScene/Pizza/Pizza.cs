using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class PizzaData
{
    public string doughID = string.Empty;
    public int bakeCount = 0;
    public List<float> cutData = new List<float>();
    public Dictionary<string, List<Vector2>> ToppingCount = new Dictionary<string, List<Vector2>>();
}

public class Pizza : MonoBehaviour, IPointable, IDragable
{
    public PizzaData PizzaData { get; private set; }

    public DrawIngredient sourceLayer;
    public DrawIngredient cheeseLayer;
    public ToppingLayer topping;

    private Transform currentSocket;
    private Transform tempSocket;

    private GameManager gameManager;

    private bool Baked { get { return PizzaData.bakeCount > 0; } }

    private void Awake()
    {
        PizzaData = new PizzaData();
    }

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController")?.GetComponent<GameManager>();
    }

    public void SetSocket(Transform go)
    {
        currentSocket = go;
    }

    public void Click(Vector2 point, PizzaCommand command)
    {
        switch (command)
        {
            case PizzaCommand.Source:
                sourceLayer.DrawPoint(point);
                break;
            case PizzaCommand.Cheese:
                cheeseLayer.DrawPoint(point);
                break;
            case PizzaCommand.Pepperoni:
            case PizzaCommand.Topping:
                topping.AddTopping(point);
                break;
        }
    }

    public void OnDragEnd()
    {
        if (tempSocket != null
            && tempSocket != currentSocket)
        {
            currentSocket.GetComponent<IPizzaSocket>()?.ClearPizza();
            currentSocket = tempSocket;
            currentSocket.GetComponent<IPizzaSocket>()?.SetPizza(transform);
            tempSocket = null;
        }
        else
        {
            transform.position = currentSocket.position;
        }
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
        if (tempSocket != null)
        {
            Debug.Log("SocketExit");
            tempSocket = null;
        }
    }

    public void OnPressObject(Vector2 position)
    {
        if (gameManager.PizzaCommand == PizzaCommand.Pepperoni)
        {
            topping.AddTopping(position);
        }
    }

    public void OnDrag(Vector2 pos, Vector2 deltaPos)
    {
        switch (gameManager.PizzaCommand)
        {
            case PizzaCommand.Source:
                sourceLayer.DrawPoint(pos);
                break;
            case PizzaCommand.Cheese:
                cheeseLayer.DrawPoint(pos);
                break;
            case PizzaCommand.Drag:
                transform.position += (Vector3)deltaPos;
                break;
        }
    }

    public void SetCurrentSocket(Transform sock)
    {
        currentSocket = sock;
    }
}
