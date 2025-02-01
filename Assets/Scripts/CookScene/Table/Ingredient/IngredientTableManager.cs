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

        pizzaSlots[0].gameObject.SetActive(true);
        pizzaSlots[1].gameObject.SetActive(true);

        foreach (var data in dicttype[IngredientTable.Type.Source])
        {
            switch (data.ingredientID)
            {
                case "tomato":
                    sourceTubs[0].Init(data);
                    break;
            }
        }

        foreach (var data in dicttype[IngredientTable.Type.Cheese])
        {
            switch (data.ingredientID)
            {
                case "cheese":
                    cheeseTub.Init(data);
                    break;
            }
        }

        int length = dicttype[IngredientTable.Type.Ingredient].Length;
        int cnt = 0;
        for (int i = 0; i < length; ++i)
        {
            if (unlockdata.ContainsKey(dicttype[IngredientTable.Type.Ingredient][i].ingredientID)
                && unlockdata[dicttype[IngredientTable.Type.Ingredient][i].ingredientID])
            {
                var tub = Instantiate(tubPrefab, ingredientTubs[cnt]);
                tub.Init(dicttype[IngredientTable.Type.Ingredient][i]);
                ++cnt;
            }
        }
    }
}
