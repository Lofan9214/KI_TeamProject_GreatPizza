using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TutorialDrawer : MonoBehaviour
{
    public DrawIngredient drawLayer;

    private Vector3 pointPosition = new Vector3(0.85f, 1.24f);

    public void DrawBrush()
    {
        drawLayer.DrawPoint(transform.TransformPoint(pointPosition));
    }

    public void ClearLayer()
    {
        drawLayer.ClearLayer();
    }
}
