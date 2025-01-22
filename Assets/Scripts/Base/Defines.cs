
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
        "StringTableEn",
        "StringTableJp",
    };
}

public static class Variables
{
    public static Languages currentLanguage = Languages.Korean;
}

public enum PizzaCommand
{
    None,
    Drag,
    Source,
    Cheese,
    Pepperoni,
    Topping
}