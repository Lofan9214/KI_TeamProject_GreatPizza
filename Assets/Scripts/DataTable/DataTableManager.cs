using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataTableManager
{
    private static readonly Dictionary<string,DataTable> tables = new Dictionary<string, DataTable>();

    static DataTableManager()
    {
#if UNITY_EDITOR
        foreach(var id in DataTableIds.String)
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
}
