using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour, IClickable, IDragable
{
    public Pizza parent;

    public void OnDrag(Vector3 pos, Vector3 deltaPos)
    {
        parent.OnDragFromBoard(pos, deltaPos);
    }

    public void OnDragEnd(Vector3 pos, Vector3 deltaPos)
    {
        parent.OnDragEnd(pos, deltaPos);
    }

    public void OnPressObject(Vector2 position)
    {
        parent.OnPressObject(position);
    }
}
