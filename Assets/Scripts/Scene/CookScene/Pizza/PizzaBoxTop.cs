using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PizzaBoxTop : MonoBehaviour, IDragable
{
    private PizzaBox box;

    public enum State
    {
        Movable,
        Immovable,
    }

    private State state;

    private void Start()
    {
        box = transform.parent.parent.GetComponent<PizzaBox>();

        state = State.Immovable;
    }

    public void OnDrag(Vector3 pos, Vector3 deltaPos)
    {
        if (box.CurrentPizza != null
            && state == State.Movable)
        {
            box.transform.position += deltaPos;
        }
    }

    public void OnDragEnd(Vector3 pos, Vector3 deltaPos)
    {
        if (state == State.Movable)
        {
            box.DragEnd();
        }
    }

    public void SetState(State state)
    {
        this.state = state;
    }
}
