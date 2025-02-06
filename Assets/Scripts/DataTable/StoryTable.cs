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
        public int groupID { get; set; }
    }

    private Dictionary<int, Data> dict = new Dictionary<int, Data>();

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
        if (!dict.ContainsKey(day))
        {
            return null;
        }

        return dict.Values.Where(p => p.day == day).ToList();
    }
}
