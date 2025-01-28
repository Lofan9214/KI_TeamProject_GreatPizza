using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPizzaSlot
{
    public bool IsSettable { get; }
    public bool IsEmpty { get; }

    public Pizza CurrentPizza { get; }

    public void ClearPizza();

    public void SetPizza(Pizza go);
}
