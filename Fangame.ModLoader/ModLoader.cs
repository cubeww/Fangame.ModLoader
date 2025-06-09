using System.Diagnostics;
using Fangame.ModLoader.Common;
using Fangame.ModLoader.GM8;
using UndertaleModLib;

namespace Fangame.ModLoader;

public class ModLoader
{
    public ModManager ModManager { get; }

    public string RunningDirectory { get; }
    public string ExecutablePath { get; private set; }
    public string ExecutableDirectory { get; private set; }
    public string GameDataPath { get; private set; }
    public string RunningExecutablePath { get; private set; }
    public string RunningGameDataPath { get; private set; }

    public ExecutableEngine ExecutableEngine { get; private set; }
    public bool IsSingleRuntimeExecutable { get; private set; }
    public UndertaleData? UndertaleData { get; private set; }
    public GM8Data? GM8Data { get; private set; }
    public CommonData? CommonData { get; private set; }

    public string[] ModNames { get; }
    public List<Mod> Mods { get; }
    public bool IsEmbeddedGameData { get; private set; }
    public string RunningArguments { get; private set; }

    public ModLoader(string executablePath, string runningDirectory, ModManager modManager, string[] modNames)
    {
        ModManager = modManager;
        RunningDirectory = runningDirectory;
        ExecutablePath = executablePath;
        ExecutableDirectory = Path.GetDirectoryName(executablePath) ?? "";
        GameDataPath = "";
        RunningExecutablePath = "";
        RunningExecutablePath = "";
        RunningGameDataPath = "";
        RunningArguments = "";
        ModNames = modNames;
        Mods = [];
    }

    public void ModAndRun()
    {
        Console.WriteLine($"executable: {ExecutablePath}");
        CleanRunningDirectory();
        AnalyzeExecutable();
        ParseGame();
        CreateModInstances();
        ModGame();
        SaveGame();
        RunGame();
    }

    private void CleanRunningDirectory()
    {
        DirectoryInfo di = new DirectoryInfo(RunningDirectory);
        if (!di.Exists)
        {
            di.Create();
            return;
        }

        foreach (FileInfo file in di.GetFiles())
        {
            file.Delete();
        }
        foreach (DirectoryInfo dir in di.GetDirectories())
        {
            dir.Delete(true);
        }
    }

    private void AnalyzeExecutable()
    {
        string[] importNames = PE.GetImportNames(ExecutablePath);
        if (importNames.Contains("Cabinet.dll"))
        {
            IsSingleRuntimeExecutable = true;
            Process p = Process.Start(new ProcessStartInfo
            {
                FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7za.exe"),
                Arguments = $"e \"{ExecutablePath}\" -o\"{RunningDirectory}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }) ?? throw new IOException("Cannot start 7za.exe process.");
            p.WaitForExit();
            if (p.ExitCode != 0)
            {
                throw new IOException("Cannot unpack cab file.");
            }
            string[] newExecutablePaths = Directory.GetFiles(RunningDirectory, "*.exe");
            if (newExecutablePaths.Length == 0)
            {
                throw new IOException("The unpacked cab file does not contain any executable files.");
            }
            ExecutablePath = newExecutablePaths[0];
            ExecutableDirectory = Path.GetDirectoryName(ExecutablePath)!;
            AnalyzeExecutable();
            return;
        }

        if (importNames.Contains("d3d8.dll"))
        {
            ExecutableEngine = ExecutableEngine.GameMaker8;
            GameDataPath = ExecutablePath;
            RunningExecutablePath = Path.Combine(RunningDirectory, Path.GetFileName(ExecutablePath));
            RunningGameDataPath = RunningExecutablePath;
            IsEmbeddedGameData = true;
        }
        else if (importNames.Contains("d3dx9_43.dll") || importNames.Contains("d3d11.dll"))
        {
            ExecutableEngine = ExecutableEngine.GameMakerStudio;
            string maybeGameDataPath = Path.Combine(ExecutableDirectory, "data.win");
            if (File.Exists(maybeGameDataPath))
            {
                GameDataPath = maybeGameDataPath;
                RunningExecutablePath = Path.Combine(RunningDirectory, Path.GetFileName(ExecutablePath));
                RunningGameDataPath = Path.Combine(RunningDirectory, "data.win");
                RunningArguments = $"-game \"{RunningGameDataPath}\"";
            }
            else
            {
                GameDataPath = ExecutablePath;
                RunningExecutablePath = Path.Combine(RunningDirectory, Path.GetFileName(ExecutablePath));
                RunningGameDataPath = RunningExecutablePath;
                IsEmbeddedGameData = true;
                Console.WriteLine("warning: data.win is embedded. most mods may fail to load.");
            }
        }
        else
        {
            ExecutableEngine = ExecutableEngine.Unknown;
            GameDataPath = ExecutablePath;
            RunningExecutablePath = Path.Combine(RunningDirectory, Path.GetFileName(ExecutablePath));
            RunningGameDataPath = RunningExecutablePath;
            IsEmbeddedGameData = false;
        }

        Console.WriteLine($"engine: {Enum.GetName(ExecutableEngine)}");
    }

    private void ParseGame()
    {
        Console.WriteLine("parsing game...");

        if (ExecutableEngine == ExecutableEngine.GameMaker8)
        {
            GM8Data = GM8Data.FromExecutable(GameDataPath);
            CommonData = new CommonData(GM8Data);
        }
        else if (ExecutableEngine == ExecutableEngine.GameMakerStudio)
        {
            if (!IsEmbeddedGameData)
            {
                using (var fs = File.OpenRead(GameDataPath))
                {
                    UndertaleData = UndertaleIO.Read(fs);
                }
                CommonData = new CommonData(UndertaleData);
            }
        }
    }

    private void CreateModInstances()
    {
        foreach (string modName in ModNames)
        {
            if (ModManager.CreateInstance(modName) is { } mod)
            {
                Mods.Add(mod);
                mod._modLoader = this;
                mod._modManager = ModManager;
                mod.ModName = modName;
                mod.ModDirectory = Path.Combine(ModManager.ModsDirectory, modName);
            }
        }
    }

    private void ModGame()
    {
        foreach (Mod mod in Mods)
        {
            Console.WriteLine($"modding: {mod.GetType().Assembly.GetName().Name}");
            mod.Load();
            CommonData?.FlushReplaceQueue();
        }
    }

    private void SaveGame()
    {
        Console.WriteLine("saving modded game...");
        if (ExecutableEngine == ExecutableEngine.GameMaker8)
        {
            GM8Data?.SaveExecutable(RunningExecutablePath);
        }
        else if (ExecutableEngine == ExecutableEngine.GameMakerStudio)
        {
            if (!IsEmbeddedGameData)
            {
                using FileStream fs = File.Create(RunningGameDataPath);
                UndertaleIO.Write(fs, UndertaleData);
            }
            if (!IsSingleRuntimeExecutable)
            {
                CopyFilesRecursively(ExecutableDirectory, RunningDirectory);
            }
        }
        else
        {
            if (ExecutablePath != RunningExecutablePath)
            {
                File.Copy(ExecutablePath, RunningExecutablePath, true);
            }
        }

        foreach (var mod in Mods)
        {
            mod.ModExecutable();
        }
    }

    private static void CopyFilesRecursively(string sourcePath, string targetPath)
    {
        foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
        }

        foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
        {
            string toPath = newPath.Replace(sourcePath, targetPath);
            if (!File.Exists(toPath))
            {
                File.Copy(newPath, toPath);
            }
        }
    }

    private void RunGame()
    {
        Console.WriteLine("running modded game...");
        Process.Start(new ProcessStartInfo
        {
            FileName = RunningExecutablePath,
            WorkingDirectory = ExecutableDirectory,
            UseShellExecute = false,
            Arguments = RunningArguments,
        });
    }
}
