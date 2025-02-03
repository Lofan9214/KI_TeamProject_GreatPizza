using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashBin : MonoBehaviour, IPizzaSlot
{
    public bool IsSettable => true;

    public bool IsEmpty => CurrentPizza == null;

    public Pizza CurrentPizza { get; protected set; }

    public void ClearPizza()
    {
    }

    public void SetPizza(Pizza go)
    {
        Destroy(go.gameObject);
    }
}
