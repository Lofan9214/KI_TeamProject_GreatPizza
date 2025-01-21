using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringTable : DataTable
{
    public class Data
    {
        public string Id { get; set; }
        public string String { get; set; }
    }

    private Dictionary<string, string> dict = new Dictionary<string, string>();

    public override void Load(string fileName)
    {
        var path = string.Format(FormatPath, fileName);
        var textAsset = Resources.Load<TextAsset>(path);
        var list = LoadCsv<Data>(textAsset.text);

        dict.Clear();

        foreach (var item in list)
        {
            if (!dict.ContainsKey(item.Id))
            {
                dict.Add(item.Id, item.String);
            }
            else
            {
                Debug.Log($"Key Duplicated: {item.Id}");
            }
        }
    }

    public string Get(string key)
    {
        if (string.IsNullOrEmpty(key) || !dict.ContainsKey(key))
        {
            return "NULL";
        }
        return dict[key];
    }
}
