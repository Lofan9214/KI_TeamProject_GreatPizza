using System;
using System.Collections;
using System.Collections.Generic;
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
    protected bool isSelected;

    protected void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController")?.GetComponent<IngameGameManager>();
    }

    public void Init(IngredientTable.Data data)
    {
        ingredient = data.ingredientID;
        if (animator.runtimeAnimatorController == null)
        {
            animator.runtimeAnimatorController = Resources.Load<AnimatorOverrideController>(string.Format(animatorFormat, ingredient));
        }
        type = data.type;
    }

    public virtual void OnPressObject(Vector2 position)
    {
        if (gameManager.PizzaCommand == ingredient)
        {
            gameManager.SetPizzaCommand(null, string.Empty, IngredientTable.Type.None);
            SetSelected(false);
            return;
        }
        gameManager.SetPizzaCommand(this, ingredient, type);
        SetSelected(true);
    }

    public void SetSelected(bool isSelected)
    {
        animator.SetBool(selected, isSelected);
        this.isSelected = isSelected;
    }
}
