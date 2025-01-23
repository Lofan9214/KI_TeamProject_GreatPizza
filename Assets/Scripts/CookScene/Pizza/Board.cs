using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour, IDragable
{
    public Pizza parent;

    public void OnDrag(Vector3 pos, Vector3 deltaPos)
    {
        parent.Move(deltaPos);
    }

    public void OnDragEnd()
    {
        parent.OnDragEnd();
    }
}
