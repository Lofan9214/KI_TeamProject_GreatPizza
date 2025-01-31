using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IngredientTableManager : MonoBehaviour
{
    public IngredientTub tubPrefab;
    public PizzaSlot[] pizzaSlots;
    public IngredientTub[] sourceTubs;
    public IngredientTub cheeseTub;
    public DoughTub doughTub;
    public Transform[] ingredientTubs;

    public void Init()
    {
        var dicttype = DataTableManager.IngredientTable.GetList().GroupBy(p => p.type).ToDictionary(p => p.Key, p => p.OrderBy(p => p.stringID).ToArray());
        var unlockdata = SaveLoadManager.Data.unlocks;
        doughTub.Init(true, new string[] { "dough" }); // ToDo юс╫ц

        foreach (var data in dicttype[2])
        {
            switch (data.ingredientID)
            {
                case "cheese":
                    cheeseTub.Init(data);
                    break;
                case "tomato":
                    sourceTubs[0].Init(data);
                    break;
            }
        }

        int length = dicttype[3].Length;
        int cnt = 0;
        for (int i = 0; i < length; ++i)
        {
            if (unlockdata.ContainsKey(dicttype[3][i].ingredientID)
                && unlockdata[dicttype[3][i].ingredientID])
            {
                var tub = Instantiate(tubPrefab, ingredientTubs[cnt]);
                tub.Init(dicttype[3][i]);
                ++cnt;
            }
        }
    }
}
