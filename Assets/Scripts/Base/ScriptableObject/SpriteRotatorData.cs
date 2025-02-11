using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/SpriteRotatorData", fileName = "Sprite Rotator Data")]
public class SpriteRotatorData : ScriptableObject
{
    public Sprite[] sprites;
    public SpriteRotator.Rotate[] rotates;
}

