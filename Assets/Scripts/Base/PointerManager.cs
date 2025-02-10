using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerManager : MonoBehaviour
{
    public LayerMask layerMask;
    public LayerMask screenMask;
    public Transform virtualCam;

    private bool hitDragable;
    private Transform target;
    private IDragable targetDragable;

    private IngameGameManager gameManager;

    public bool enableCamDrag;

    private int screenLockLayer;

    private void Awake()
    {
        enableCamDrag = false;
        gameManager = GetComponent<IngameGameManager>();
        screenLockLayer = LayerMask.NameToLayer("ScreenLock");
    }

    private void Update()
    {
#if UNITY_ANDROID || UNITY_IOS
        RaycastHit2D hit;
        if (MultiTouchManager.Instance.IsTouchStart)
        {
            hitDragable = false;
            var touchposition = MultiTouchManager.Instance.TouchPosition;
            var screenpoint = Camera.main.ScreenToWorldPoint(touchposition);
            hit = RaycastScreen(screenpoint);

            if (hit)
            {
                IClickable pointable = hit.collider.GetComponent<IClickable>();
                pointable?.OnPressObject(hit.point);
                target = hit.collider.transform;
                targetDragable = target.GetComponent<IDragable>();
                hitDragable = targetDragable != null;
            }
        }
        else if (MultiTouchManager.Instance.IsMoving)
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(MultiTouchManager.Instance.TouchPosition);
            Vector3 deltaWorldPos = worldPos - Camera.main.ScreenToWorldPoint(MultiTouchManager.Instance.TouchPosition - MultiTouchManager.Instance.DeltaPosition);

            hit = RaycastScreen(worldPos);

            worldPos.z = 0f;
            deltaWorldPos.z = 0f;
            if (hitDragable)
            {
                if ((hit && (hit.collider.transform == target)) || !hit)
                    targetDragable.OnDrag(worldPos, deltaWorldPos);
                else if (hit && hit.collider.gameObject.layer == screenLockLayer)
                    targetDragable.OnDragEnd(worldPos, deltaWorldPos);
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
            if (hitDragable)
            {
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(MultiTouchManager.Instance.TouchPosition);
                Vector3 deltaWorldPos = worldPos - Camera.main.ScreenToWorldPoint(MultiTouchManager.Instance.TouchPosition - MultiTouchManager.Instance.DeltaPosition);

                hit = RaycastScreen(worldPos);
                if ((hit && hit.collider.transform == target) || !hit)
                {
                    targetDragable.OnDragEnd(worldPos, deltaWorldPos);
                }
            }
        }
#endif
    }

    private RaycastHit2D RaycastScreen(Vector2 worldPos)
    {
        float minDepth = -Mathf.Infinity;
        var hit = Physics2D.Raycast(worldPos, Vector2.zero, float.PositiveInfinity, screenMask);
        if (hit)
        {
            minDepth = -0.5f;
        }
        hit = Physics2D.Raycast(worldPos, Vector2.zero, float.PositiveInfinity, layerMask, minDepth);
        return hit;
    }
}
