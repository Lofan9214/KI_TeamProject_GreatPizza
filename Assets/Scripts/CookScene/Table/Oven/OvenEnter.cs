using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvenEnter : MonoBehaviour, IPizzaSocket
{
    public OvenExit ovenExit;

    public bool IsSettable => true;
    public bool IsEmpty => CurrentPizza == null;
    public Pizza CurrentPizza { get; private set; }

    public void ClearPizza()
    {
        CurrentPizza = null;
    }

    public void SetPizza(Pizza go)
    {
        CurrentPizza = go;
        CurrentPizza.transform.position = transform.position;

        StartCoroutine(Cooking());
    }

    private IEnumerator Cooking()
    {
        CurrentPizza.PizzaState = Pizza.State.Immovable;
        do
        {
            yield return new WaitForSeconds(0.5f);
        } while (!ovenExit.IsEmpty);
        CurrentPizza.PizzaState = Pizza.State.Movable;
        CurrentPizza.Roast();
        ovenExit.SetPizza(CurrentPizza);
        ClearPizza();
    }
}
