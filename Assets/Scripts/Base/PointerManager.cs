using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerManager : MonoBehaviour
{
    public LayerMask layerMask;
    public Transform virtualCam;

    private bool hitPointable;
    private Transform target;

    private IngameGameManager gameManager;

    

    private void Awake()
    {
        gameManager = GetComponent<IngameGameManager>();
    }

    private void Update()
    {
#if UNITY_EDITOR

#endif
#if UNITY_ANDROID || UNITY_IOS
        RaycastHit2D hit;
        if (MultiTouchManager.Instance.IsTouchStart)
        {
            var touchposition = MultiTouchManager.Instance.TouchPosition;
            hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(touchposition), Vector2.zero, float.PositiveInfinity, layerMask);
            hitPointable = hit;

            if (hit)
            {
                IClickable pointable = hit.collider.GetComponent<IClickable>();
                pointable?.OnPressObject(hit.point);
                target = hit.collider.transform;
                //if(hit.collider.CompareTag("PizzaBoard"))
                //{
                //    gameManager.PizzaCommand = PizzaCommand.Drag;
                //}
            }
        }
        else if (MultiTouchManager.Instance.IsMoving)
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(MultiTouchManager.Instance.TouchPosition);
            Vector3 deltaWorldPos = worldPos - Camera.main.ScreenToWorldPoint(MultiTouchManager.Instance.TouchPosition - MultiTouchManager.Instance.DeltaPosition);
            worldPos.z = 0f;
            deltaWorldPos.z = 0f;
            if (hitPointable)
            {
                IDragable dragable = target?.GetComponent<IDragable>();
                if (dragable != null)
                {
                    dragable.OnDrag(worldPos, deltaWorldPos);
                }
            }
            else
            {
                Vector3 cameraPos = Camera.main.transform.position;
                cameraPos.x -= deltaWorldPos.x;
                virtualCam.position = cameraPos;
            }
        }
        else if (MultiTouchManager.Instance.IsTouchEnd)
        {
            if (hitPointable)
            {
                IDragable dragable = target?.GetComponent<IDragable>();
                if (dragable != null)
                {
                    dragable.OnDragEnd();
                }
            }
        }
#endif
    }
}
