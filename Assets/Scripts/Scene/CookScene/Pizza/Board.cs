using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour, IClickable, IDragable
{
    public Pizza parent;

    public BoxCollider2D boxCollider { get; private set; }

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    public void OnDrag(Vector3 pos, Vector3 deltaPos)
    {
        parent.OnDragFromBoard(pos, deltaPos);
    }

    public void OnDragEnd(Vector3 pos, Vector3 deltaPos)
    {
        parent.DragEndSlot(pos, deltaPos);
    }

    public void OnPressObject(Vector2 position)
    {
        parent.OnPressObject(position);
    }
}
