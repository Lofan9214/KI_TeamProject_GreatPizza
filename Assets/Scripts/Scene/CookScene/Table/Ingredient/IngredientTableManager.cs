using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IngredientTableManager : MonoBehaviour
{
    public IngredientVat tubPrefab;
    public PizzaSlot[] pizzaSlots;
    public IngredientVat[] sourceTubs;
    public IngredientVat cheeseTub;
    public PepperoniVat pepperoniTub;
    public DoughTub doughTub;
    public Transform[] ingredientTubs;
    public SpriteRenderer[] tableSprites;
    public float[] tablesLengthes;
    public Transform trashCan;
    public PolygonCollider2D camConfiningCollider;

    public void Init()
    {
        var dicttype = DataTableManager.IngredientTable.GetList().GroupBy(p => p.type).ToDictionary(p => p.Key, p => p.OrderBy(p => p.stringID).ToArray());
        var unlockdata = SaveLoadManager.Data.ingredients;
        doughTub.Init(true, new string[] { "dough" }); // ToDo юс╫ц

        pizzaSlots[0].gameObject.SetActive(true);

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
            if (dicttype[IngredientTable.Type.Ingredient][i].ingredientID == "pepperoni")
            {
                pepperoniTub.Init(dicttype[IngredientTable.Type.Ingredient][i]);
                if (!unlockdata.ContainsKey(dicttype[IngredientTable.Type.Ingredient][i].ingredientID)
                || !unlockdata[dicttype[IngredientTable.Type.Ingredient][i].ingredientID])
                {
                    pepperoniTub.SetLocked();
                }
                continue;
            }

            if (unlockdata.ContainsKey(dicttype[IngredientTable.Type.Ingredient][i].ingredientID)
                && unlockdata[dicttype[IngredientTable.Type.Ingredient][i].ingredientID])
            {
                var tub = Instantiate(tubPrefab, ingredientTubs[cnt]);
                tub.Init(dicttype[IngredientTable.Type.Ingredient][i]);
                ++cnt;
            }
        }

        float tableLength = tablesLengthes[0];
        bool extended = false;
        if (cnt > 0)
        {
            tableLength = tablesLengthes[1];
            extended = true;
        }
        else if (cnt > 3)
        {
            tableLength = tablesLengthes[2];
        }

        for (int i = 0; i < tableSprites.Length; ++i)
        {
            var size = tableSprites[i].size;
            var pos = tableSprites[i].transform.localPosition;
            size.x = (i == tableSprites.Length - 1) ? tableLength - 0.2f : tableLength;
            pos.x = tableLength * 0.5f;
            tableSprites[i].size = size;
            tableSprites[i].transform.localPosition = pos;
        }

        if (extended)
        {
            var offset = Vector3.left * (tableLength - tablesLengthes[0]);
            transform.Translate(offset);
            trashCan.Translate(offset);
            var path = camConfiningCollider.GetPath(0);
            path[0] += (Vector2)offset;
            path[1] += (Vector2)offset;
            camConfiningCollider.SetPath(0, path);
        }
    }
}
