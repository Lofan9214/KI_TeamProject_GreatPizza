using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutter : MonoBehaviour, IClickable, IDragable
{
    public PizzaSlot currentTable;
    public Transform cutterObject;

    private bool isCutting = false;

    public void OnPressObject(Vector2 position)
    {
        if(currentTable.IsEmpty)
        {
            return;
        }
        isCutting = true;
        cutterObject.gameObject.SetActive(true);
    }

    public void OnDrag(Vector3 pos, Vector3 deltaPos)
    {
        if (isCutting)
        {
            Vector2 offset = pos - cutterObject.transform.position;

            if (offset.sqrMagnitude < 1f)
            {
                isCutting = false;
                cutterObject.gameObject.SetActive(false);
                return;
            }
            var dir = offset.normalized;
            cutterObject.transform.up = dir;
        }
    }

    public void OnDragEnd(Vector3 pos, Vector3 deltaPos)
    {
        if (!isCutting)
        {
            return;
        }

        currentTable.CurrentPizza.Cut(cutterObject.rotation);
        isCutting = false;
        cutterObject.gameObject.SetActive(false);
    }
}
