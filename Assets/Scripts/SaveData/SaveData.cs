using System.Collections.Generic;
using System.Linq;

public abstract class SaveData
{
    public int Version { get; protected set; }

    public abstract SaveData VersionUp();
}

public class SaveDataV1 : SaveData
{
    public float budget = 100f;
    public int slotusage = 1;
    public int days = 0;
    public Dictionary<string, bool> ingredients = new Dictionary<string, bool>();
    public Dictionary<string, bool> upgrades = new Dictionary<string, bool>();

    public SaveDataV1()
    {
        Version = 1;
    }

    public override SaveData VersionUp()
    {
        return this;
    }

    public SaveDataV1 DeepCopy()
    {
        var saveData = new SaveDataV1();

        saveData.budget = budget;
        saveData.slotusage = slotusage;
        saveData.days = days;
        saveData.ingredients = ingredients.ToDictionary(p => p.Key, p => p.Value);
        saveData.upgrades = upgrades.ToDictionary(p => p.Key, p => p.Value);

        return saveData;
    }

    public void Set(SaveDataV1 data)
    {
        budget = data.budget;
        slotusage = data.slotusage;
        days = data.days;
        ingredients = data.ingredients.ToDictionary(p => p.Key, p => p.Value);
        upgrades = data.upgrades.ToDictionary(p => p.Key, p => p.Value);
    }
}