using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToppingLayer : MonoBehaviour
{
    public Topping toppingPrefab;

    private List<Topping> toppings = new List<Topping>();

    public void AddTopping(GameObject topping, Vector2 position, IngredientTable.Data toppingData)
    {
        topping.transform.parent = transform;
        topping.transform.position = position;
        topping.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0, 360f));

        var toppingcomponent = topping.GetComponent<Topping>();
        toppingcomponent.SetData(toppingData);
        toppingcomponent.AddOrderOffset(toppings.Count);

        toppings.Add(toppingcomponent);
    }

    public void Clear()
    {
        foreach (var topping in toppings)
        {
            topping.Release();
        }
        toppings.Clear();
    }
}
