
public enum Languages
{
    Korean,
    English,
    Japanese,
}

public class DataTableIds
{
    public static readonly string[] String =
    {
        "StringTableKr",
        //"StringTableEn",
        //"StringTableJp",
    };

    public static readonly string Recipe = "RecipeTable";

    public static readonly string Ingredient = "IngredientTable";

    public static readonly string Talk = "TalkTable";

    public static readonly string NPC = "NPCTable";
}

public class Variables
{
    public static Languages currentLanguage = Languages.Korean;
}

public enum PizzaCommand
{
    None,
    Source,
    Cheese,
    Topping,
}

public enum InGamePlace
{
    Hall,
    Kitchen
}
