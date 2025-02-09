using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutter : MonoBehaviour, IClickable, IDragable
{
    public CuttingSlot currentTable;
    public Transform cutterObject;

    private bool isCutting = false;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnPressObject(Vector2 position)
    {
        if (currentTable.IsEmpty)
        {
            return;
        }
        isCutting = true;
        cutterObject.gameObject.SetActive(true);
        Vector2 offset = position - (Vector2)cutterObject.transform.position;
        var dir = offset.normalized;
        cutterObject.transform.up = dir;
        spriteRenderer.enabled = false;
        currentTable.CurrentPizza.CircleCollider.enabled = false;
    }

    public void OnDrag(Vector3 pos, Vector3 deltaPos)
    {
        if (isCutting)
        {
            Vector2 offset = cutterObject.transform.position - pos;

            if (offset.sqrMagnitude < 1f)
            {
                isCutting = false;
                cutterObject.gameObject.SetActive(false);
                spriteRenderer.enabled = true;
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

        currentTable.CurrentPizza.Cut(cutterObject.rotation * Quaternion.Euler(0f, 0f, 90f));
        isCutting = false;
        cutterObject.gameObject.SetActive(false);
        spriteRenderer.enabled = true;
        currentTable.CurrentPizza.CircleCollider.enabled = true;
    }
}
