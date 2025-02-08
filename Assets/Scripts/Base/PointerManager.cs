using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerManager : MonoBehaviour
{
    public LayerMask layerMask;
    public LayerMask screenMask;
    public Transform virtualCam;

    private bool hitPointable;
    private Transform target;

    private IngameGameManager gameManager;

    public bool enableCamDrag;

    private void Awake()
    {
        enableCamDrag = false;
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
            var screenpoint = Camera.main.ScreenToWorldPoint(touchposition);
            float minDepth = -Mathf.Infinity;
            hit = Physics2D.Raycast(screenpoint, Vector2.zero, float.PositiveInfinity, screenMask);
            if(hit)
            {
                minDepth = -0.5f;
            }
            hit = Physics2D.Raycast(screenpoint, Vector2.zero, float.PositiveInfinity, layerMask, minDepth);
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
            else if (enableCamDrag)
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
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(MultiTouchManager.Instance.TouchPosition);
                Vector3 deltaWorldPos = worldPos - Camera.main.ScreenToWorldPoint(MultiTouchManager.Instance.TouchPosition - MultiTouchManager.Instance.DeltaPosition);

                IDragable dragable = target?.GetComponent<IDragable>();
                if (dragable != null)
                {
                    dragable.OnDragEnd(worldPos, deltaWorldPos);
                }
            }
        }
#endif
    }
}
