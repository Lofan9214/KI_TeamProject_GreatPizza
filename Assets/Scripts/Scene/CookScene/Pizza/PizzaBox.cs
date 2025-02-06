using UnityEngine;

public class PizzaBox : MonoBehaviour, IPizzaSlot
{
    private static readonly int CloseTop = Animator.StringToHash("CloseTop");
    private static readonly int Complete = Animator.StringToHash("Complete");

    private Animator animator;
    public Transform boxPosition;
    public IngameGameManager gameManager;
    public PizzaBoxTop boxTop;
    public Transform box;

    private Transform currentSlot;
    private Transform tempSlot;

    public bool IsSettable => true;
    public bool IsEmpty => CurrentPizza == null;
    public Pizza CurrentPizza { get; private set; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<IngameGameManager>();
    }

    public void ClearPizza()
    {
        CurrentPizza = null;
    }

    public void SetPizza(Pizza go)
    {
        CurrentPizza = go;
        CurrentPizza.transform.parent = box;
        CurrentPizza.transform.localPosition = Vector3.zero;
        animator.SetTrigger(CloseTop);
        CookComplete();
    }

    public void CookComplete()
    {
        animator.SetTrigger(Complete);
    }


    private void Hall()
    {
        gameManager.hall.SetSlot(this);
        gameManager.ChangePlace(InGamePlace.Hall);
        boxTop.SetState(PizzaBoxTop.State.Movable);
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
        {
            transform.position = currentSlot.position;
            return;
        }
        tempSlot.GetComponent<NPC>().SetPizza(CurrentPizza);
        Destroy(gameObject);
    }
}
