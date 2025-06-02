using System.Text.Json;
using Fangame.ModLoader.Common;
using Fangame.ModLoader.GM8;
using UndertaleModLib;

namespace Fangame.ModLoader;

public abstract class Mod
{
    private readonly static JsonSerializerOptions ConfigSerializerOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
    };

    internal Modder Modder = null!;
    public string ModDirectory { get; internal set; } = "";
    public string ModsDirectory => Modder.ModsDirectory;
    public string RunningDirectory => Modder.RunningDirectory;
    public string ExecutablePath => Modder.ExecutablePath;
    public string ExecutableDirectory => Modder.ExecutableDirectory;
    public string GameDataPath => Modder.GameDataPath;
    public string RunningExecutablePath => Modder.RunningExecutablePath;
    public string RunningGameDataPath => Modder.RunningGameDataPath;
    public ExecutableEngine ExecutableEngine => Modder.ExecutableEngine;
    public bool IsSingleRuntimeExecutable => Modder.IsSingleRuntimeExecutable;
    public bool IsEmbeddedGameData => Modder.IsEmbeddedGameData;
    public UndertaleData? UndertaleData => Modder.UndertaleData;
    public GM8Data? GM8Data => Modder.GM8Data;
    public CommonData? CommonData => Modder.CommonData;

    public virtual void Load()
    {
    }

    public virtual void ModExecutable()
    {
    }

    public T LoadConfig<T>(T defaultValue, string configFileName = "Config.json")
    {
        string configFilePath = Path.Combine(ModsDirectory, configFileName);
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
        string sourceFilePath = Path.Combine(ModDirectory, fileName);
        string destFilePath = Path.Combine(RunningDirectory, fileName);
        File.Copy(sourceFilePath, destFilePath, true);
    }
}
