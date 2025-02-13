using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IngredientVat : MonoBehaviour, IClickable
{
    protected IngameGameManager gameManager;

    protected readonly int selected = Animator.StringToHash("Selected");
    protected const string animatorFormat = "Animators/{0}";
    public Animator animator;
    protected string ingredient;
    protected IngredientTable.Type type;
    protected SpriteRenderer spriteRenderer;
    public bool isSelected { get; protected set; }
    protected bool isUpgraded;
    public SpriteRotator trayRotator;
    public SpriteRotatorData trayData;

    protected void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController")?.GetComponent<IngameGameManager>();
    }

    public void Init(IngredientTable.Data data)
    {
        ingredient = data.ingredientID;

        isUpgraded = SaveLoadManager.Data.upgrades[DataTableManager.StoreTable.GetIdFromIngredientId(data.ingredientID)];
        if (isUpgraded)
        {
            trayRotator.targetSprite = trayData.sprites[1];
            trayRotator.rotate = trayData.rotates[1];
        }
        else
        {
            trayRotator.targetSprite = trayData.sprites[0];
            trayRotator.rotate = trayData.rotates[0];
        }
        trayRotator.RotateSprite();

        if (animator.runtimeAnimatorController == null)
        {
            animator.runtimeAnimatorController = Resources.Load<AnimatorOverrideController>(string.Format(animatorFormat, ingredient));
        }
        type = data.type;
    }

    public virtual void OnPressObject(Vector2 position)
    {
        if (isUpgraded && gameManager.npc.Recipe.ingredientIds.Contains(ingredient))
        {
            var pizza = gameManager.kitchen.ingredientTable.pizzaSlots[0].CurrentPizza;
            if (pizza != null && !isSelected)
            {
                switch (type)
                {
                    case IngredientTable.Type.Source:
                    case IngredientTable.Type.Cheese:
                        pizza.StartCoroutine(pizza.AutoDraw(ingredient));
                        break;
                    case IngredientTable.Type.Ingredient:
                        pizza.StartCoroutine(pizza.AutoIngredient(ingredient));
                        break;
                }
            }
        }
        if (isSelected)
        {
            gameManager.SetPizzaCommand(null, string.Empty, IngredientTable.Type.None);
            SetSelected(false);
            return;
        }
        if (!isUpgraded)
            gameManager.SetPizzaCommand(this, ingredient, type);
        else
            gameManager.SetPizzaCommand(this, string.Empty, IngredientTable.Type.None);
        SetSelected(true);
    }

    public void SetSelected(bool isSelected)
    {
        animator.SetBool(selected, isSelected);
        this.isSelected = isSelected;
    }
}
