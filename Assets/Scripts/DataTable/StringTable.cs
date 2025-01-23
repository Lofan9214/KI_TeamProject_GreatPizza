using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringTable : DataTable
{
    public class Data
    {
        public int stringID { get; set; }
        public string line { get; set; }
    }

    private Dictionary<int, string> dict = new Dictionary<int, string>();

    public override void Load(string fileName)
    {
        var path = string.Format(FormatPath, fileName);
        var textAsset = Resources.Load<TextAsset>(path);
        var list = LoadCsv<Data>(textAsset.text);

        dict.Clear();

        foreach (var item in list)
        {
            if (!dict.ContainsKey(item.stringID))
            {
                dict.Add(item.stringID, item.line);
            }
            else
            {
                Debug.Log($"Key Duplicated: {item.stringID}");
            }
        }
    }

    public string Get(int key)
    {
        if (!dict.ContainsKey(key))
        {
            return "NULL";
        }
        return dict[key];
    }
}
