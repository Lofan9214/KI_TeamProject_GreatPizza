using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoughLoaf : MonoBehaviour, IClickable
{
    public bool IsExist => gameObject.activeSelf;

    public UnityEvent<DoughLoaf> OnClick;

    public string DoughId { get; private set; }

    private SpriteRenderer renderer;

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
    }

    public void Set(string id)
    {
        DoughId = id;
        renderer.sprite = DataTableManager.IngredientTable.Get(id).SpriteLoaf;
    }

    public void OnPressObject(Vector2 position)
    {
        OnClick?.Invoke(this);
    }
}
