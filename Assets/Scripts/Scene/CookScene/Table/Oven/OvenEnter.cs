using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OvenEnter : MonoBehaviour, IPizzaSlot
{
    public OvenMid nextTarget;

    public bool IsSettable => true;
    public bool IsEmpty => CurrentPizza == null;
    public Pizza CurrentPizza { get; private set; }

    private float cookTime = 5f;

    private void Start()
    {
        var data = DataTableManager.StoreTable.GetTypeList(StoreTable.Type.SpeedyOven)[0];
        if (SaveLoadManager.Data.upgrades[data.storeID])
        {
            cookTime *= 0.5f;
        }
    }

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
        CurrentPizza.CurrentState = Pizza.State.Roasting;
        CurrentPizza.Movable = false;
        yield return new WaitUntil(() => nextTarget.IsEmpty);

        float timer = 0f;

        while (timer < cookTime)
        {
            timer += Time.deltaTime;
            CurrentPizza.transform.position = Vector3.Lerp(transform.position, nextTarget.transform.position, timer / cookTime);
            yield return null;
        }

        CurrentPizza.Roast();
        nextTarget.SetPizza(CurrentPizza);
        ClearPizza();
    }
}
