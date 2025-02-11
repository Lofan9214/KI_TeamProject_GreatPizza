using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StoreTable : DataTable
{
    public enum Type
    {
        None,
        LongerDay,
        SpeedyOven,
        LayerBuddy,
        CuttingBuddy,
        ToppingBuddy,
    }

    public class Data
    {
        public string storeID { get; set; }
        public int NameID { get; set; }
        public int descriptionID { get; set; }
        public Type type { get; set; }
        public string atribute { get; set; }
        public int price { get; set; }

        public SpriteRotatorData spriteRotatorData;
    }

    private const string spriteDataFormat = "SpriteDatas/Store/{0}";
    private Dictionary<string, Data> dict = new Dictionary<string, Data>();

    public override void Load(string fileName)
    {
        var path = string.Format(FormatPath, fileName);
        var textAsset = Resources.Load<TextAsset>(path);
        var list = LoadCsv<Data>(textAsset.text);

        dict.Clear();

        foreach (var item in list)
        {
            if (!dict.ContainsKey(item.storeID))
            {

                dict.Add(item.storeID, item);

                item.spriteRotatorData = Resources.Load<SpriteRotatorData>(string.Format(spriteDataFormat, item.storeID));
            }
            else
            {
                Debug.Log($"Key Duplicated: {item.storeID}");
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

    public List<Data> GetTypeList(Type type)
    {
        return dict.Values.Where(p => p.type == type).ToList();
    }
}
