using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoughTub : MonoBehaviour, IClickable
{
    public PizzaSocket target;
    public Pizza pizzaPrefab;

    public void OnPressObject(Vector2 position)
    {
        if (target.IsEmpty)
        {
            var ps = Instantiate(pizzaPrefab);
            ps.SetCurrentSocket(target.transform);
            target.SetPizza(ps);
        }
    }
}
