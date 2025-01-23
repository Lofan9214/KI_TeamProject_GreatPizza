using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvenExit : MonoBehaviour, IPizzaSocket
{
    public bool IsSettable => false;

    public bool IsEmpty => CurrentPizza == null;

    public Pizza CurrentPizza { get; private set; }

    public void ClearPizza()
    {
        CurrentPizza = null;
    }

    public void SetPizza(Pizza go)
    {
        go.SetCurrentSocket(transform);
        CurrentPizza = go;
        CurrentPizza.transform.position = transform.position;
    }
}
