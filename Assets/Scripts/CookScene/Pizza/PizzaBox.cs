using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PizzaBox : MonoBehaviour, IPizzaSlot
{
    private static readonly int CloseTop = Animator.StringToHash("CloseTop");
    private static readonly int Complete = Animator.StringToHash("Complete");

    private Animator animator;
    public Transform boxPosition;
    public IngameGameManager gameManager;

    private Transform currentSlot;
    private Transform tempSlot;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<IngameGameManager>();
    }

    public bool IsSettable => true;
    public bool IsEmpty => CurrentPizza == null;
    public Pizza CurrentPizza { get; private set; }

    public void ClearPizza()
    {
        CurrentPizza = null;
    }

    public void SetPizza(Pizza go)
    {
        CurrentPizza = go;
        CurrentPizza.transform.parent = boxPosition;
        CurrentPizza.box = this;
        CurrentPizza.transform.localPosition = Vector3.zero;
        animator.SetTrigger(CloseTop);
        CookComplete();
    }

    public void CookComplete()
    {
        animator.SetTrigger(Complete);
    }

    private void DestroyThis()
    {
        Destroy(gameObject);
    }

    private void Hall()
    {
        boxPosition.position = Vector3.zero;

        gameManager.hall.SetSlot(this);
        gameManager.ChangePlace(true);
    }

    public void SetCurrentSlot(Transform slot)
    {
        currentSlot = slot;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("NPC"))
        {
            Debug.Log($"SlotFound{collision.gameObject.name}");
            tempSlot = collision.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (tempSlot != null
            && tempSlot == collision.transform)
        {
            Debug.Log("SlotExit");
            tempSlot = null;
        }
    }

    public void DragEnd()
    {
        if (tempSlot == null)
            return;
        tempSlot.GetComponent<NPCBehaviour>().SetPizza(CurrentPizza);
        DestroyThis();
    }
}
