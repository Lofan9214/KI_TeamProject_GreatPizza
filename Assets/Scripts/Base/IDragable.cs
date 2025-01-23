using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDragable
{
    void OnDrag(Vector3 pos,Vector3 deltaPos);
    void OnDragEnd();
}
