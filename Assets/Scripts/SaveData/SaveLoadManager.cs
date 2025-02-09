using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using SaveDataVC = SaveDataV1; // 세이브데이터 버전 변경 시 수정해야될 곳

public static class SaveLoadManager
{
    // 세이브데이터 버전 변경 시 수정해야될 곳    
    public static int CurrentSaveDataVersion { get; private set; } = 1;

    public static SaveDataVC Data { get; set; }

    private static JsonSerializerSettings setting = new JsonSerializerSettings
    {
        Formatting = Formatting.Indented,
        TypeNameHandling = TypeNameHandling.All,
    };

    private static readonly string SaveFileName = "SaveAuto.json";

    private static string SaveDirectory
    {
        get
        {
            return $"{Application.persistentDataPath}/Save";
        }
    }

    static SaveLoadManager()
    {
        if (!Load())
        {
            Data = new SaveDataVC();
            Save();
        }
    }

    public static bool Save(int slot = 0)
    {
        if (Data == null)
        {
            return false;
        }

        if (!Directory.Exists(SaveDirectory))
        {
            Directory.CreateDirectory(SaveDirectory);
        }

        var path = Path.Combine(SaveDirectory, SaveFileName);
        var json = JsonConvert.SerializeObject(Data, setting);

        File.WriteAllText(path, json);
        return true;
    }

    public static bool Load(int slot = 0)
    {
        if (!Directory.Exists(SaveDirectory))
        {
            return false;
        }

        var path = Path.Combine(SaveDirectory, SaveFileName);

        if (!File.Exists(path))
        {
            return false;
        }

        var json = File.ReadAllText(path);

        var saveData = JsonConvert.DeserializeObject<SaveData>(json, setting);

        while (saveData.Version < CurrentSaveDataVersion)
        {
            saveData = saveData.VersionUp();
        }

        Data = saveData as SaveDataVC;

        return true;
    }
}
