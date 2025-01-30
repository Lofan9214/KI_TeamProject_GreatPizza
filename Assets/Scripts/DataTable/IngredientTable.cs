using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;
using static RecipeTable;

//ingredientID, stringID, price, profit, success, fail, image
//tomato,110101,0.6,1.0,0,0,
//cheeze,110102,0.6,1.0,0,0,

public class IngredientTable : DataTable
{
    public class Data
    {
        public string ingredientID { get; set; }
        public int type { get; set; }
        public int stringID { get; set; }
        public float price { get; set; }
        public float profit { get; set; }
        public int success { get; set; }
        public int fail { get; set; }
        public float happy_min { get; set; }
        public float happy_max { get; set; }
        public float normal_min { get; set; }
        public float normal_max { get; set; }
        public string image { get; set; }

        public Sprite Sprite
        {
            get
            {
                return Resources.Load<Sprite>(string.Format(spriteFormat, image));
            }
        }

        public Sprite SpriteLoaf
        {
            get
            {
                if (type == 1)
                {
                    return Resources.Load<Sprite>(string.Format(loafFormat, image));
                }
                return null;
            }
        }

        public Sprite SpriteTub
        {
            get
            {
                if (type == 2 || type == 3)
                {
                    return Resources.Load<Sprite>(string.Format(tubFormat, image));
                }
                return null;
            }
        }
    }

    private const string spriteFormat = "Sprite/Pizza/{0}";
    private const string loafFormat = "Sprite/Pizza/Loaf/{0}";
    private const string tubFormat = "Sprite/Pizza/Tub/{0}";

    private Dictionary<string, Data> dict = new Dictionary<string, Data>();

    public override void Load(string fileName)
    {
        var path = string.Format(FormatPath, fileName);
        var textAsset = Resources.Load<TextAsset>(path);
        var list = LoadCsv<Data>(textAsset.text);

        dict.Clear();

        foreach (var item in list)
        {
            if (!dict.ContainsKey(item.ingredientID))
            {
                dict.Add(item.ingredientID, item);
            }
            else
            {
                Debug.Log($"Key Duplicated: {item.ingredientID}");
            }
        }
    }

    public Data Get(string key)
    {
        if (!dict.ContainsKey(key))
        {
            return null;
        }

        return dict[key];
    }


    public List<Data> GetList()
    {
        return dict.Values.ToList();
    }
}
