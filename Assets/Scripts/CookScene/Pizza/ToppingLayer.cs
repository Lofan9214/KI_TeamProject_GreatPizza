using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToppingLayer : MonoBehaviour
{
    public ToppingData[] toppingData;
    public Topping toppingPrefab;

    private List<Topping> toppings = new List<Topping>();

    public void AddTopping(Vector2 position)
    {
        var topping = Instantiate(toppingPrefab, position, Quaternion.identity, transform);
        topping.SetData(toppingData[0]);
        topping.AddOrderOffset(toppings.Count);
        toppings.Add(topping);
    }
}
