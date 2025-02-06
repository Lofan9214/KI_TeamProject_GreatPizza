using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/IngredientSpriteData", fileName = "Ingredient Sprite Data")]
public class IngredientSpriteData : ScriptableObject
{
    public Sprite[] toppingSprites;
    public Sprite storeSprite;
    public Sprite storeTray;
}
