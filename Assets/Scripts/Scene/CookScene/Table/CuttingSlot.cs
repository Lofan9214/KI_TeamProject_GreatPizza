using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingSlot : MonoBehaviour , IPizzaSlot
{
    public bool Settable = false;

    public bool IsSettable => Settable;

    public bool IsEmpty => CurrentPizza == null;

    public Pizza CurrentPizza { get; private set; }

    public void ClearPizza()
    {
        CurrentPizza = null;
    }

    public void SetPizza(Pizza go)
    {
        Debug.Log($"SetPizzaToSocket{name}");
        CurrentPizza = go;
        CurrentPizza.transform.position = transform.position;

        if (Settable && go.CurrentState == Pizza.State.AddingTopping)
        {
            go.CurrentState = Pizza.State.Movable;
        }
    }
}
