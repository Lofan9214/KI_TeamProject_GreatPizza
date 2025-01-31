using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;
using Random = UnityEngine.Random;

public class RecipeTable : DataTable
{
    public class Data
    {
        public int recipeID { get; set; }
        public int stringID { get; set; }
        public int roastcutting { get; set; }
        public string dough { get; set; }
        public string ingredientID { get; set; }
        public string[] ingredientIds => ingredientID.Split('_');
        public int roast => roastcutting / 10 % 10;
        public int cutting => roastcutting % 10;
    }

    private Dictionary<int, Data> dict = new Dictionary<int, Data>();

    private Func<Data, bool> randomFilter;

    public override void Load(string fileName)
    {
        var path = string.Format(FormatPath, fileName);
        var textAsset = Resources.Load<TextAsset>(path);
        var list = LoadCsv<Data>(textAsset.text);

        dict.Clear();

        foreach (var item in list)
        {
            if (!dict.ContainsKey(item.recipeID))
            {

                dict.Add(item.recipeID, item);
            }
            else
            {
                Debug.Log($"Key Duplicated: {item.recipeID}");
            }
        }
    }

    public Data Get(int key)
    {
        if (!dict.ContainsKey(key))
        {
            return null;
        }

        return dict[key];
    }

    public Data RandomGet()
    {
        randomFilter ??= p =>
            {
                bool contains = true;
                foreach (var id in p.ingredientIds)
                {
                    if (string.IsNullOrEmpty(id))
                    {
                        continue;
                    }
                    if (!SaveLoadManager.Data.unlocks.TryGetValue(id, out bool unlocked) || !unlocked)
                    {
                        contains = false;
                        break;
                    }
                }
                return contains;
            };

        List<Data> filtered = dict.Values.Where(randomFilter).ToList();

        int randomindex = Random.Range(0, filtered.Count);
        return filtered[randomindex];
    }

    public List<Data> GetList()
    {
        return dict.Values.ToList();
    }
}
