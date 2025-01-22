using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PizzaData
{
    public string doughID = string.Empty;
    public int bakeCount = 0;
    public List<float> cutData = new List<float>();
    public Dictionary<string, List<Vector2>> ToppingCount = new Dictionary<string, List<Vector2>>();
}

public class Pizza : MonoBehaviour, IPointable
{
    public enum DoughLayer
    {
        None,
        Source,
        Cheese,
        Pepperoni,
        Topping
    }

    public PizzaData PizzaData { get; private set; }

    public DrawIngredient sourceLayer;
    public DrawIngredient cheeseLayer;
    public ToppingLayer topping;
    public LayerMask layerMask;

    private Collider2D collider2d;
    private DoughLayer selectedLayer;

    private Vector3 lastPosition;
    private bool baked { get { return PizzaData.bakeCount > 0; } }

    private void Awake()
    {
        collider2d = GetComponent<Collider2D>();
        PizzaData = new PizzaData();
    }

    private void Start()
    {
        SetLayer(DoughLayer.None.ToString());
        lastPosition = transform.position;
    }

    public void OnPressObject(Vector2 point)
    {
        if (selectedLayer == DoughLayer.Topping)
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, float.PositiveInfinity, layerMask);

            if (hit && hit.collider == collider2d)
            {
                topping.AddTopping(hit.point);
            }
        }
    }

    public void DragPizza(Vector2 point, string ingredient, bool down = false)
    {
        DoughLayer result = DoughLayer.None;
        if (string.IsNullOrEmpty(ingredient)
            || !Enum.TryParse(ingredient, out result))
        {
            return;
        }
        if (baked)
        {
            transform.position = point;
            lastPosition = point;
            return;
        }
        switch (result)
        {
            case DoughLayer.Source:
                sourceLayer.DrawPoint(point);
                break;
            case DoughLayer.Cheese:
                cheeseLayer.DrawPoint(point);
                break;
            case DoughLayer.Pepperoni:
            case DoughLayer.Topping:
                if (down)
                {
                    topping.AddTopping(point);
                }
                break;
        }
    }

    public void SetLayer(string layer)
    {
        if (!Enum.TryParse(layer, out DoughLayer result))
        {
            return;
        }
        selectedLayer = result;

        sourceLayer.enabled = false;
        cheeseLayer.enabled = false;

        switch (result)
        {
            case DoughLayer.Source:
                sourceLayer.enabled = true;
                break;
            case DoughLayer.Cheese:
                cheeseLayer.enabled = true;
                break;
        }
    }
}
