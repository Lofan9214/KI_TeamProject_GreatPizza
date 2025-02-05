using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientTub : MonoBehaviour, IClickable
{
    private IngameGameManager gameManager;

    public Animator animator;
    private string ingredient;
    private IngredientTable.Type type;
    private SpriteRenderer spriteRenderer;
    private bool isOn;

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
        if (gameManager.PizzaCommand == ingredient)
        {
            gameManager.SetPizzaCommand(null, string.Empty, IngredientTable.Type.None);
            animator.SetBool("Selected", false);
            Off();
            return;
        }
        gameManager.SetPizzaCommand(this, ingredient, type);
            animator.SetBool("Selected", true);
    }

    public void Off()
    {
        isOn = false;
    }
}
