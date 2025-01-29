using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DoughTub : MonoBehaviour
{
    public PizzaSlot target;
    public Pizza pizzaPrefab;

    public DoughLoaf[] loafs;
    private Dictionary<string, List<DoughLoaf>> loafbytype = new Dictionary<string, List<DoughLoaf>>();

    private void Start()
    {
        Set(new string[] { "dough" });
    }

    private void Set(string[] doughs)
    {
        int each = loafs.Length / doughs.Length;
        int count = 0;
        for (int i = 0; i < doughs.Length; ++i)
        {
            for (int j = 0; j < each; ++j)
            {
                loafs[count].Set(doughs[i]);
                if (loafbytype.ContainsKey(doughs[i]))
                {
                    loafbytype[doughs[i]].Add(loafs[count]);
                }
                else
                {
                    loafbytype.Add(doughs[i], new List<DoughLoaf>() { loafs[count] });
                }
                loafs[count].OnClick.AddListener(OnPressDoughLoaf);
                ++count;
            }
        }
        for (int i = count; i < loafs.Length; ++i)
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
            ps.SetDough("dough");
            ps.SetCurrentSlot(target.transform);
            target.SetPizza(ps);
            sender.gameObject.SetActive(false);
        }

        if (loafbytype[sender.DoughId].Where(p => p.IsExist).Count() == 0)
        {
            foreach(var loaf in loafbytype[sender.DoughId])
            {
                loaf.gameObject.SetActive(true);
            }
        }
    }
}
