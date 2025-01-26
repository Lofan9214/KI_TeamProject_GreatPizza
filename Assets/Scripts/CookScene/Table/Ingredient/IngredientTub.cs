using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientTub : MonoBehaviour, IClickable
{
    IngameGameManager gameManager;

    public void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController")?.GetComponent<IngameGameManager>();
    }

    public void OnPressObject(Vector2 position)
    {
        gameManager.SetPizzaCommand(gameObject.name);
    }
}
