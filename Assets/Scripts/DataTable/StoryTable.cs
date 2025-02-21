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
        public string image { get; set; }
    }

    private Dictionary<int, List<Data>> dict = new Dictionary<int, List<Data>>();

    public override void Load(string fileName)
    {
        var path = string.Format(FormatPath, fileName);
        var textAsset = Resources.Load<TextAsset>(path);
        var list = LoadCsv<Data>(textAsset.text);

        dict.Clear();

        foreach (var item in list)
        {
            if (!dict.ContainsKey(item.day))
            {
                dict.Add(item.day, new List<Data>() { item });
            }
            else
            {
                dict[item.day].Add(item);
            }
        }
    }

    public List<Data> GetDatas()
    {
        return dict.SelectMany(p => p.Value).ToList();
    }

    public bool IsExistData(int day) => dict.ContainsKey(day) && dict[day].Count > 0;

    public List<Data> GetAtDay(int day)
    {
        if (!dict.ContainsKey(day))
        {
            return null;
        }

        return dict[day];
    }
}
