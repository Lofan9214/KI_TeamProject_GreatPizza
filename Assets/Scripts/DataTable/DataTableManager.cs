using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataTableManager
{
    private static readonly Dictionary<string, DataTable> tables = new Dictionary<string, DataTable>();

    static DataTableManager()
    {
#if UNITY_EDITOR
        foreach (var id in DataTableIds.String)
        {
            var table = new StringTable();
            table.Load(id);
            tables.Add(id, table);
        }
#else
        var table = new StringTable();
        var stringTableId = DataTableIds.String[(int)Variables.currentLanguage];
        table.Load(stringTableId);
        tables.Add(stringTableId, table);
#endif

        var recipeTable = new RecipeTable();
        recipeTable.Load(DataTableIds.Recipe);
        tables.Add(DataTableIds.Recipe, recipeTable);

        var ingredientTable = new IngredientTable();
        ingredientTable.Load(DataTableIds.Ingredient);
        tables.Add(DataTableIds.Ingredient, ingredientTable);

        var talkTable = new TalkTable();
        talkTable.Load(DataTableIds.Talk);
        tables.Add(DataTableIds.Talk, talkTable);

        var npcTable = new NPCTable();
        npcTable.Load(DataTableIds.NPC);
        tables.Add(DataTableIds.NPC, npcTable);

        var storyTable = new StoryTable();
        storyTable.Load(DataTableIds.Story);
        tables.Add(DataTableIds.Story, storyTable);

        var storeTable = new StoreTable();
        storeTable.Load(DataTableIds.Store);
        tables.Add(DataTableIds.Store, storeTable);
    }

    public static T Get<T>(string id) where T : DataTable
    {
        if (!tables.ContainsKey(id))
        {
            Debug.LogError("Table Not Exists");
            return null;
        }
        return tables[id] as T;
    }

    public static StringTable StringTable
    {
        get
        {
            return Get<StringTable>(DataTableIds.String[(int)Variables.currentLanguage]);
        }
    }

    public static RecipeTable RecipeTable
    {
        get
        {
            return Get<RecipeTable>(DataTableIds.Recipe);
        }
    }

    public static IngredientTable IngredientTable
    {
        get
        {
            return Get<IngredientTable>(DataTableIds.Ingredient);
        }
    }

    public static TalkTable TalkTable
    {
        get
        {
            return Get<TalkTable>(DataTableIds.Talk);
        }
    }

    public static NPCTable NPCTable
    {
        get
        {
            return Get<NPCTable>(DataTableIds.NPC);
        }
    }

    public static StoryTable StoryTable
    {
        get
        {
            return Get<StoryTable>(DataTableIds.Story);
        }
    }

    public static StoreTable StoreTable
    {
        get
        {
            return Get<StoreTable>(DataTableIds.Store);
        }
    }
}
