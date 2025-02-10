using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvenExit : MonoBehaviour, IPizzaSlot
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
        go.SetCurrentSlot(transform);
        CurrentPizza = go;
        CurrentPizza.CurrentState = Pizza.State.Roasted;
        CurrentPizza.Movable = true;
        CurrentPizza.transform.position = transform.position;
    }
}
