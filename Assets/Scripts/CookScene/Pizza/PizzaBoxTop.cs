using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PizzaBoxTop : MonoBehaviour, IDragable
{
    private PizzaBox box;

    private void Start()
    {
        box = transform.parent.parent.GetComponent<PizzaBox>();
    }

    public void OnDrag(Vector3 pos, Vector3 deltaPos)
    {
        if (box.CurrentPizza != null)
        {
            box.transform.position += deltaPos;
        }
    }

    public void OnDragEnd(Vector3 pos, Vector3 deltaPos)
    {
        box.DragEnd();
    }
}
