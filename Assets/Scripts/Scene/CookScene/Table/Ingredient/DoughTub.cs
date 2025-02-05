using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DoughTub : MonoBehaviour
{
    public PizzaSlot target;
    public Pizza pizzaPrefab;

    public DoughLoaf[] loafs;
    public Transform tray;

    private Dictionary<string, List<DoughLoaf>> loafbytype = new Dictionary<string, List<DoughLoaf>>();

    public Vector3 fullscale;
    public Vector3 fullposition;
    public Vector3 halfscale;
    public Vector3 halfposition;

    public Sprite[] doughs;

    private IngameGameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<IngameGameManager>();
    }

    public void Init(bool fullSize, string[] doughs)
    {
        int start;
        int end = loafs.Length;

        if (fullSize)
        {
            start = 0;
            //tray.localScale = fullscale;
            //tray.localPosition = fullposition;
        }
        else
        {
            start = end / 2;
            //tray.localScale = halfscale;
            //tray.localPosition = halfposition;
        }

        for (int i = 0; i < end / 2; ++i)
        {
            loafs[i].gameObject.SetActive(i >= start);
        }

        int each = (end - start) / doughs.Length;
        int index = start;
        for (int i = 0; i < doughs.Length; ++i)
        {
            for (int j = 0; j < each; ++j)
            {
                loafs[index].Set(doughs[i]);
                if (loafbytype.ContainsKey(doughs[i]))
                {
                    loafbytype[doughs[i]].Add(loafs[index]);
                }
                else
                {
                    loafbytype.Add(doughs[i], new List<DoughLoaf>() { loafs[index] });
                }
                loafs[index].OnClick.AddListener(OnPressDoughLoaf);
                ++index;
            }
        }
        for (int i = index; i < loafs.Length; ++i)
        {
            loafs[i].Set(doughs[doughs.Length - 1]);
            loafs[i].OnClick.AddListener(OnPressDoughLoaf);
        }
    }


    private void OnPressDoughLoaf(DoughLoaf sender)
    {
        if (target.IsEmpty)
        {
            var ps = Instantiate(pizzaPrefab);
            ps.SetDough(sender.DoughId);
            gameManager.tempSaveData.budget -= DataTableManager.IngredientTable.Get(sender.DoughId).price;
            gameManager.uiManager.UpdateCurrentBudget();
            ps.SetCurrentSlot(target.transform);
            target.SetPizza(ps);
            sender.gameObject.SetActive(false);
        }

        if (loafbytype[sender.DoughId].Where(p => p.IsExist).Count() == 0)
        {
            foreach (var loaf in loafbytype[sender.DoughId])
            {
                loaf.gameObject.SetActive(true);
            }
        }
    }
}
