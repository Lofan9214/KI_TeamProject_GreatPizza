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
    public float currency = 100f;
    public int slotusage = 1;
    public Dictionary<string, bool> unlocks = new Dictionary<string, bool>();
    public Dictionary<int, bool> upgrades = new Dictionary<int, bool>();

    public SaveDataV1()
    {
        Version = 1;
    }

    public override SaveData VersionUp()
    {
        return this;
    }
}