using Fangame.ModLoader.Common;
using Fangame.ModLoader.GM8;
using UndertaleModLib;

namespace Fangame.ModLoader;

public abstract class Mod
{
    internal ModLoader ModLoader = null!;
    internal ModManager ModManager = null!;

    public string ModName { get; internal set; } = null!;
    public string ModDirectory { get; internal set; } = null!;
    public string ModsDirectory => ModManager.ModsDirectory;
    public string RunningDirectory => ModLoader.RunningDirectory;
    public string ExecutablePath => ModLoader.ExecutablePath;
    public string ExecutableDirectory => ModLoader.ExecutableDirectory;
    public string GameDataPath => ModLoader.GameDataPath;
    public string RunningExecutablePath => ModLoader.RunningExecutablePath;
    public string RunningGameDataPath => ModLoader.RunningGameDataPath;
    public ExecutableEngine ExecutableEngine => ModLoader.ExecutableEngine;
    public bool IsSingleRuntimeExecutable => ModLoader.IsSingleRuntimeExecutable;
    public bool IsEmbeddedGameData => ModLoader.IsEmbeddedGameData;
    public UndertaleData? UndertaleData => ModLoader.UndertaleData;
    public GM8Data? GM8Data => ModLoader.GM8Data;
    public CommonData? CommonData => ModLoader.CommonData;

    public virtual void Load()
    {
    }

    public virtual void ModExecutable()
    {
    }

    public void CopyFileToRunningDirectory(string fileName)
    {
        string sourceFilePath = Path.Combine(ModDirectory, fileName);
        string destFilePath = Path.Combine(RunningDirectory, fileName);
        File.Copy(sourceFilePath, destFilePath, true);
    }

    public T GetConfig<T>() where T : ModConfig
    {
        return (T)ModManager.GetConfig(ModName)!;
    }
}
