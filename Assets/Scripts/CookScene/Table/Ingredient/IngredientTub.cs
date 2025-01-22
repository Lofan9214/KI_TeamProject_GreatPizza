using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientTub : MonoBehaviour, IPointable
{
    GameManager gameManager;

    public void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController")?.GetComponent<GameManager>();
    }

    public void OnPressObject(Vector2 position)
    {
        if (Enum.TryParse(gameObject.name, out PizzaCommand result))
        {
            Debug.Log(result);
            gameManager.PizzaCommand = result;
        }
    }
}
