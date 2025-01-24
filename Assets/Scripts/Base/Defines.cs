
public enum Languages
{
    Korean,
    English,
    Japanese,
}

public static class DataTableIds
{
    public static readonly string[] String =
    {
        "StringTableKr",
        //"StringTableEn",
        //"StringTableJp",
    };

    public static readonly string Recipe = "RecipeTable";

    public static readonly string Ingredient = "IngredientTable";
}

public static class Variables
{
    public static Languages currentLanguage = Languages.Korean;
}

public enum PizzaCommand
{
    None,
    Source,
    Cheese,
    Pepperoni,
    Sausage,
    Topping
}