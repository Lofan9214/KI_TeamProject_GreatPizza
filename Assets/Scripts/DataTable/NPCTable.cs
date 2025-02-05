using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCTable : DataTable
{
    public class Data
    {
        public int npcID { get; set; }
        public int type { get; set; }
        public string Image { get; set; }

        public Sprite Sprite => Resources.Load<Sprite>(string.Format(sprite, Image));

        public GameObject Prefab;
    }

    private Dictionary<int, Data> dict = new Dictionary<int, Data>();
    private const string sprite = "Sprite/NPC/{0}";
    private const string prefab = "Prefabs/{0}";

    public override void Load(string fileName)
    {
        var path = string.Format(FormatPath, fileName);
        var textAsset = Resources.Load<TextAsset>(path);
        var list = LoadCsv<Data>(textAsset.text);

        dict.Clear();

        foreach (var item in list)
        {
            if (!dict.ContainsKey(item.npcID))
            {
                item.Prefab = Resources.Load<GameObject>(string.Format(prefab, item.Image));
                dict.Add(item.npcID, item);
            }
            else
            {
                Debug.Log($"Key Duplicated: {item.npcID}");
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

    public Data GetRandom(int type)
    {
        var list = dict.Values.Where(p => p.type == type).ToList();

        return list[Random.Range(0, list.Count)];
    }
}
