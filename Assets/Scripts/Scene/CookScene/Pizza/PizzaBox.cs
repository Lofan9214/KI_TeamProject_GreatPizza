using UnityEngine;

public class PizzaBox : MonoBehaviour, IPizzaSlot
{
    private static readonly int closeTopHash = Animator.StringToHash("CloseTop");
    private static readonly int completeHash = Animator.StringToHash("Complete");
    private static readonly int resetHash = Animator.StringToHash("Reset");

    private Animator animator;
    public Transform boxPosition;
    public IngameGameManager gameManager;
    public PizzaBoxTop boxTop;
    public Transform box;

    private Transform currentSlot;
    private Transform tempSlot;
    private AudioSource audioSource;

    public bool IsSettable => true;
    public bool IsEmpty => CurrentPizza == null;
    public Pizza CurrentPizza { get; private set; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
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
        CurrentPizza.transform.SetParent(box);
        CurrentPizza.transform.localPosition = Vector3.zero;
        animator.SetTrigger(closeTopHash);
    }

    public void CookComplete()
    {
        audioSource.Play();
        animator.SetTrigger(completeHash);
    }

    public void ResetState()
    {
        CurrentPizza = null;
        boxTop.SetState(PizzaBoxTop.State.Immovable);
        animator.SetTrigger(resetHash);
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
        var npc = tempSlot.GetComponent<NPC>();
        if (npc != null)
        {
            npc.SetPizza(CurrentPizza);
            gameObject.SetActive(false);
        }
    }
}
