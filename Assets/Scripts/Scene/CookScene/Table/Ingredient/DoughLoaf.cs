using UnityEngine;
using UnityEngine.Events;

public class DoughLoaf : MonoBehaviour, IClickable
{
    public bool IsExist => gameObject.activeSelf;

    public UnityEvent<DoughLoaf> OnClick;

    public string DoughId { get; private set; }

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Set(string id)
    {
        DoughId = id;
        spriteRenderer.sprite = DataTableManager.IngredientTable.Get(id).SpriteLoaf;
    }

    public void OnPressObject(Vector2 position)
    {
        OnClick?.Invoke(this);
    }
}
