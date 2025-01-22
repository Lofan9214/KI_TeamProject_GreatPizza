using UnityEngine;

public class PizzaSocket : MonoBehaviour, IPizzaSocket
{
    public Transform pizza;

    public bool IsEmpty => pizza == null;

    public void ClearPizza()
    {
        pizza = null;
    }

    public void SetPizza(Transform go)
    {
        pizza = go;
        pizza.position = transform.position;
    }
}
