using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientTableManager : MonoBehaviour
{
    private enum Selection
    {
        None,
        Source,
        Cheese,
        Pepperoni,
        Sausage,
    }

    public GameObject pizzaDough;
    public Transform doughSocket;

    public void SpreadDough()
    {
        var go = Instantiate(pizzaDough);
        go.transform.position = doughSocket.position;
    }
}
