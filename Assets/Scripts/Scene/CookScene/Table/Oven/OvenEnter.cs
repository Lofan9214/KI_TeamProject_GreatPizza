using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvenEnter : MonoBehaviour, IPizzaSlot
{
    public OvenMid nextTarget;

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
        yield return new WaitUntil(() => nextTarget.IsEmpty);

        WaitForEndOfFrame waitframe = new WaitForEndOfFrame();

        float timer = 0f;

        while (timer < 5f)
        {
            timer += Time.deltaTime;
            CurrentPizza.transform.position = Vector3.Lerp(transform.position, nextTarget.transform.position, timer * 0.2f);
            yield return waitframe;
        }

        CurrentPizza.Roast();
        nextTarget.SetPizza(CurrentPizza);
        ClearPizza();
    }
}
