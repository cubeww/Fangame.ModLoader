using System.Text.Json;
using Fangame.ModLoader.GM8;
using UndertaleModLib;

namespace Fangame.ModLoader;

public abstract class Mod
{
    private readonly static JsonSerializerOptions ConfigSerializerOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
    };

    public Modder Modder = null!;
    public string ModDirectory = null!;

    public virtual void Load()
    {
    }

    public virtual void ModGM8(GM8Data data)
    {
    }

    public virtual void ModGMS(UndertaleData data)
    {
    }

    public virtual void ModExecutable(FileStream stream)
    {
    }

    public T LoadConfig<T>(T defaultValue, string configFileName = "Config.json")
    {
        string configFilePath = Path.Combine(Modder.ModsDirectory, configFileName);
        if (File.Exists(configFilePath))
        {
            string configJson = File.ReadAllText(configFilePath);
            return JsonSerializer.Deserialize<T>(configJson)!;
        }
        else
        {
            using FileStream fs = File.Create(configFileName);
            JsonSerializer.Serialize(fs, defaultValue, ConfigSerializerOptions);
            return defaultValue;
        }
    }

    public void CopyFileToRunningDirectory(string fileName)
    {
        string sourceFilePath = Path.Combine(Modder.ModsDirectory, fileName);
        string destFilePath = Path.Combine(Modder.RunningDirectory, fileName);
        File.Copy(sourceFilePath, destFilePath, true);
    }
}
