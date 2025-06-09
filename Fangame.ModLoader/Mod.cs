using Fangame.ModLoader.Common;
using Fangame.ModLoader.GM8;
using UndertaleModLib;

namespace Fangame.ModLoader;

public abstract class Mod
{
    internal ModLoader _modLoader = null!;
    internal ModManager _modManager = null!;

    public string ModName { get; internal set; } = null!;
    public string ModDirectory { get; internal set; } = null!;
    public string ModsDirectory => _modManager.ModsDirectory;
    public string RunningDirectory => _modLoader.RunningDirectory;
    public string ExecutablePath => _modLoader.ExecutablePath;
    public string ExecutableDirectory => _modLoader.ExecutableDirectory;
    public string GameDataPath => _modLoader.GameDataPath;
    public string RunningExecutablePath => _modLoader.RunningExecutablePath;
    public string RunningGameDataPath => _modLoader.RunningGameDataPath;
    public ExecutableEngine ExecutableEngine => _modLoader.ExecutableEngine;
    public bool IsSingleRuntimeExecutable => _modLoader.IsSingleRuntimeExecutable;
    public bool IsEmbeddedGameData => _modLoader.IsEmbeddedGameData;
    public UndertaleData? UndertaleData => _modLoader.UndertaleData;
    public GM8Data? GM8Data => _modLoader.GM8Data;
    public CommonData? CommonData => _modLoader.CommonData;

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
        return (T)_modManager.GetConfig(ModName)!;
    }
}
