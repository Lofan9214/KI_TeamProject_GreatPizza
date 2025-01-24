using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToppingLayer : MonoBehaviour
{
    public ToppingData[] toppingData;
    public Topping toppingPrefab;

    private List<Topping> toppings = new List<Topping>();

    public void AddTopping(Vector2 position, string toppingId)
    {
        var topping = Instantiate(toppingPrefab, position, Quaternion.identity, transform);
        foreach (var datum in toppingData)
        {
            if (datum.toppingId == toppingId)
            {
                topping.SetData(datum);
            }
        }
        topping.AddOrderOffset(toppings.Count);
        toppings.Add(topping);
    }
}
