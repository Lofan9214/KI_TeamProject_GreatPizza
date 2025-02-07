using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;
using static UnityEngine.LightProbeProxyVolume;

//ingredientID, stringID, price, profit, success, fail, image
//tomato,110101,0.6,1.0,0,0,
//cheeze,110102,0.6,1.0,0,0,

public class IngredientTable : DataTable
{
    public enum Type
    {
        None,
        Dough,
        Source,
        Cheese,
        Ingredient
    }

    public class Data
    {
        public string ingredientID { get; set; }
        public Type type { get; set; }
        public int stringID { get; set; }
        public float price { get; set; }
        public float profit { get; set; }
        public int success { get; set; }
        public int fail { get; set; }
        public float happy_min { get; set; }
        public float happy_max { get; set; }
        public float normal_min { get; set; }
        public float normal_max { get; set; }
        public float store_price { get; set; }
        public int day { get; set; }


        public IngredientSpriteData spriteDatas;
    }

    private const string spriteDataFormat = "SpriteDatas/{0}";

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

                item.spriteDatas = Resources.Load<IngredientSpriteData>(string.Format(spriteDataFormat, item.ingredientID));
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
