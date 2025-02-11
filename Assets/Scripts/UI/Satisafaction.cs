using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Satisafaction : MonoBehaviour
{
    public SpriteRotator rotator;
    public SpriteRotatorData spriteRotatorData;
    public int[] thresholds;

    public void SetSatisfaction(int satisfaction)
    {
        for (int i = 0; i < thresholds.Length; ++i)
        {
            if (satisfaction > thresholds[i])
            {
                rotator.targetSprite = spriteRotatorData.sprites[i];
                rotator.rotate = spriteRotatorData.rotates[i];
                break;
            }
        }
        rotator.RotateSprite();
    }
}
