using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class TalkTable : DataTable
{
    public enum TalkType
    {
        Normal = 1,
        Story = 2,
    }

    public class Data
    {
        public int talkID { get; set; }
        public int recipeID { get; set; }
        public int stringID { get; set; }
        public string groupID { get; set; }
        public int state { get; set; }
        public TalkType type { get; set; }
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
            if (!dict.ContainsKey(item.talkID))
            {
                dict.Add(item.talkID, item);
            }
            else
            {
                Debug.Log($"Key Duplicated: {item.talkID}");
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

    public List<IGrouping<string, Data>> GetByRecipeId(int recipeID)
    {
        return dict.Values.Where(p => p.recipeID == recipeID && p.type == TalkType.Normal).GroupBy(p => p.groupID).ToList();
    }

    public List<Data> GetByGroupId(string groupId)
    {
        return dict.Values.Where(p => p.groupID == groupId).ToList();
    }

    public int[] GetRandomData(int recipeID)
    {
        var list = GetByRecipeId(recipeID);
        if (list.Count > 0)
        {
            return list[Random.Range(0, list.Count)].Select(p => p.stringID).Concat(GetResultTalk()).ToArray();
        }
        return null;
    }

    public int[] GetResultTalk()
    {
        int[] result = new int[3];
        var talks = GetValueList().Where(p => p.type == TalkType.Normal).GroupBy(p => p.state).ToDictionary(p => p.Key, p => p.ToList());
        for (int i = 4, j = 0; j < result.Length; ++i, ++j)
        {
            int sss = talks[i].Count();
            int rand = Random.Range(0, talks[i].Count());
            result[j] = talks[i].ElementAt(rand).stringID;
        }
        return result;
    }

    public List<Data> GetValueList()
    {
        return dict.Values.ToList();
    }
}
