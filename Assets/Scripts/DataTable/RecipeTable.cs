using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

//recipeID,stringID,roastcutting,ingredientID
//0301,110201,10,dough
//0302,110202,13, dough_tomato_cheese

public class RecipeTable : DataTable
{
    public class RecipeData
    {
        public int recipeID;
        public int stringID;
        public int roast;
        public int cutting;
        public string dough;
        public string[] ingredientIds;
    }

    private class Data
    {
        public int recipeID { get; set; }
        public int stringID { get; set; }
        public int roastcutting { get; set; }
        public string dough { get; set; }
        public string ingredientID { get; set; }
    }

    private Dictionary<int, RecipeData> dict = new Dictionary<int, RecipeData>();

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
                RecipeData recipeData = new RecipeData()
                {
                    recipeID = item.recipeID,
                    stringID = item.stringID,
                    roast = item.roastcutting / 10 % 10,
                    cutting = item.roastcutting % 10,
                    dough = item.dough,
                    ingredientIds = item.ingredientID.Split('_')
                };

                dict.Add(item.recipeID, recipeData);
            }
            else
            {
                Debug.Log($"Key Duplicated: {item.recipeID}");
            }
        }
    }

    public RecipeData Get(int key)
    {
        if (!dict.ContainsKey(key))
        {
            return null;
        }

        return dict[key];
    }

    public RecipeData RandomGet()
    {
        Func<RecipeData, bool> filter =
            p =>
            {
                bool contains = true;
                foreach (var id in p.ingredientIds)
                {
                    if (string.IsNullOrEmpty(id))
                    {
                        continue;
                    }
                    if (!PlayerData.unlocks.Contains(id))
                    {
                        contains = false;
                        break;
                    }
                }
                return contains;
            };

        List<RecipeData> filtered = dict.Values.Where(filter).ToList();

        int randomindex = Random.Range(0, filtered.Count);
        return filtered[randomindex];
    }

    public List<RecipeData> GetList()
    {
        return dict.Values.ToList();
    }
}
