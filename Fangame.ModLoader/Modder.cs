using System.Diagnostics;
using System.Reflection;
using Fangame.ModLoader.GM8;
using UndertaleModLib;

namespace Fangame.ModLoader;


// Example - GameMaker 8
//
// Fangame.ModLoader/
//     Mods/ -- ModsDirectory
//         Iwpo/ -- ModDirectory
//     Running/ -- RunningDirectory
//         I wanna kill the kamilia 3.exe -- RunningExecutablePath / RunningGameDataPath
// IW/
//     I wanna kill the kamilia 3/ -- ExecutableDirectory
//         I wanna kill the kamilia 3.exe -- ExecutablePath / GameDataPath


// Example - GameMaker Studio
//
// Fangame.ModLoader/
//     Mods/ -- ModsDirectory
//         Iwpo/ -- ModDirectory
//     Running/ -- RunningDirectory
//         Arcfox Needle.exe -- RunningExecutablePath
//         data.win -- RunningGameDataPath
// IW/
//     Arcfox Needle/ -- ExecutableDirectory
//         Arcfox Needle.exe -- ExecutablePath
//         data.win -- GameDataPath


// Example - GameMaker Studio (Single Runtime Executable)
//
// Fangame.ModLoader/
//     Mods/ -- ModsDirectory
//         Iwpo/ -- ModDirectory
//     Running/ -- ExecutableDirectory / RunningDirectory
//         Flames Needle.exe -- ExecutablePath / RunningExecutablePath
//         data.win -- GameDataPath / RunningGameDataPath
// IW/
//     Flames Needle/
//         Flames Needle.exe


// Example - GameMaker Studio (YYC)
//
// Fangame.ModLoader/
//     Mods/ -- ModsDirectory
//         Iwpo/ -- ModDirectory
//     Running/ -- RunningDirectory
//         Crimson Needle 2.5.exe -- RunningExecutablePath / RunningGameDataPath
// IW/
//     Crimson Needle 2.5/ -- ExecutableDirectory
//         Crimson Needle 2.5.exe -- ExecutablePath / GameDataPath


public class Modder
{
    public string ModsDirectory;
    public string RunningDirectory;

    public string ExecutablePath;
    public string ExecutableDirectory;

    public string GameDataPath;
    public string RunningExecutablePath;
    public string RunningGameDataPath;

    public string RunningArguments;
    public string[] ModNames;
    public List<Mod> Mods;
    public ExecutableEngine ExecutableEngine;

    public bool IsSingleRuntimeExecutable;
    public bool IsEmbeddedGameData;

    public UndertaleData? UndertaleData;
    public GM8Data? GM8Data;

    public Modder(string executablePath, string[] modNames)
    {
        ModsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Mods");
        RunningDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Running");
        ExecutablePath = executablePath;
        ExecutableDirectory = Path.GetDirectoryName(executablePath)!;
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
        LoadMods();
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
        string[] importNames = PEUtils.GetImportNames(ExecutablePath);
        if (importNames.Contains("Cabinet.dll"))
        {
            IsSingleRuntimeExecutable = true;
            Process p = Process.Start(new ProcessStartInfo
            {
                FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7za.exe"),
                Arguments = $"e \"{ExecutablePath}\" -o\"{RunningDirectory}\"",
                RedirectStandardOutput = true,
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
                RunningArguments = $"-game {RunningGameDataPath}";
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
            throw new IOException("Not supported executable engine.");
        }

        Console.WriteLine($"engine: {Enum.GetName(ExecutableEngine)}");
    }

    private void ParseGame()
    {
        if (GameDataPath == null) return;

        Console.WriteLine("parsing game...");

        if (ExecutableEngine == ExecutableEngine.GameMaker8)
        {
            GM8Data = GM8Data.FromExecutable(GameDataPath);
        }
        else if (ExecutableEngine == ExecutableEngine.GameMakerStudio)
        {
            if (!IsEmbeddedGameData)
            {
                using var fs = File.OpenRead(GameDataPath);
                UndertaleData = UndertaleIO.Read(fs);
            }
        }
    }

    private void LoadMods()
    {
        if (!Directory.Exists(ModsDirectory))
        {
            Directory.CreateDirectory(ModsDirectory);
        }

        foreach (string modName in ModNames)
        {
            string modDirectory = Path.Combine(ModsDirectory, modName);
            Assembly modAssembly = Assembly.LoadFile(Path.Combine(modDirectory, $"{modName}.dll"));
            Type? modType = modAssembly.GetTypes().Where(x => typeof(Mod).IsAssignableFrom(x)).FirstOrDefault();
            if (modType != null)
            {
                Mod mod = (Mod)Activator.CreateInstance(modType)!;
                mod.Modder = this;
                mod.ModDirectory = modDirectory;
                Mods.Add(mod);
            }
        }
    }

    private void ModGame()
    {
        foreach (Mod mod in Mods)
        {
            Console.WriteLine($"modding: {mod.GetType().Assembly.GetName().Name}");
            mod.Load();
            if (GM8Data != null)
            {
                mod.ModGM8(GM8Data);
            }
            else if (UndertaleData != null)
            {
                mod.ModGMS(UndertaleData);
            }
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
            if (ExecutablePath != RunningExecutablePath)
            {
                File.Copy(ExecutablePath, RunningExecutablePath, true);
            }
            if (!IsEmbeddedGameData)
            {
                using FileStream fs = File.Create(RunningGameDataPath);
                UndertaleIO.Write(fs, UndertaleData);
            }
        }

        using (FileStream fs = File.OpenWrite(RunningExecutablePath))
        {
            foreach (var mod in Mods)
            {
                mod.ModExecutable(fs);
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
