using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SaveData
{
    public int Version { get; protected set; }

    public abstract SaveData VersionUp();
}

public class SaveDataV1 : SaveData
{
    public int currency = 0;
    public int slotusage = 1;
    public Dictionary<string, bool> unlocks = new Dictionary<string, bool>();


    public SaveDataV1()
    {
        Version = 1;
    }

    public override SaveData VersionUp()
    {
        return this;
    }
}