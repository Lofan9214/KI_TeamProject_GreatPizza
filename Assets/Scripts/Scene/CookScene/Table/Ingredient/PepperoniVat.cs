using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PepperoniVat : IngredientVat
{
    public GameObject lockSprite;

    public void SetLocked()
    {
        lockSprite.SetActive(true);
    }

    public override void OnPressObject(Vector2 position)
    {
        if (lockSprite.activeSelf)
        {
            return;
        }
        base.OnPressObject(position);
    }
}
