using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StoryTable : DataTable
{
    public class Data
    {
        public int story_npcID { get; set; }
        public int timelock { get; set; }
        public int satisfactionlock { get; set; }
        public int timestart { get; set; }
        public int timeend { get; set; }
        public int startState { get; set; }
        public int price { get; set; }
        public int day { get; set; }
        public int recipeID { get; set; }
        public string groupID { get; set; }
        public string prefabId { get; set; }

        public GameObject Prefab;
    }

    private Dictionary<int, Data> dict = new Dictionary<int, Data>();
    private const string prefab = "Prefabs/{0}";

    public override void Load(string fileName)
    {
        var path = string.Format(FormatPath, fileName);
        var textAsset = Resources.Load<TextAsset>(path);
        var list = LoadCsv<Data>(textAsset.text);

        dict.Clear();

        foreach (var item in list)
        {
            if (!dict.ContainsKey(item.story_npcID))
            {
                item.Prefab = Resources.Load<GameObject>(string.Format(prefab, item.prefabId));
                dict.Add(item.story_npcID, item);
            }
            else
            {
                Debug.Log($"Key Duplicated: {item.story_npcID}");
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

    public bool IsExistData(int day) => dict.Values.Where(p => p.day == day).Count() > 0;

    public List<Data> GetAtDay(int day)
    {
        var list = dict.Values.Where(p => p.day == day).ToList();
        if (list.Count == 0)
        {
            return null;
        }

        return list;
    }
}
