using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientTub : MonoBehaviour, IClickable
{
    private IngameGameManager gameManager;

    private string ingredient;
    private int type;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController")?.GetComponent<IngameGameManager>();
    }

    public void Init(IngredientTable.Data data)
    {
        ingredient = data.ingredientID;
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        spriteRenderer.sprite = data.SpriteTub;
        type = data.type;
    }

    public void OnPressObject(Vector2 position)
    {
        gameManager.SetPizzaCommand(ingredient, type);
    }
}
