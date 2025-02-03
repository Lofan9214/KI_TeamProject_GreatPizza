using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hall : MonoBehaviour
{
    public PizzaSlot slot;
    public Transform cameraStartPosition;
    private PolygonCollider2D confineCollider;

    private void Awake()
    {
        confineCollider = GetComponent<PolygonCollider2D>();
    }

    public void SetSlot(PizzaBox box)
    {
        slot.SetPizza(box.CurrentPizza);
        box.SetCurrentSlot(slot.transform);
        box.transform.position = slot.transform.position;
    }

    public void Set(CinemachineConfiner2D confiner)
    {
        confiner.m_BoundingShape2D = confineCollider;
        confiner.gameObject.transform.position = cameraStartPosition.position;
    }
}
