using System.CommandLine;
using System.Diagnostics;

namespace Fangame.ModLoader;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Fangame.ModLoader v0.1.0 by NyaCube");
        Console.WriteLine("===================================");
        var rootCommand = new RootCommand("Load the mods and run the specified game file.");
        var executablePath = new Argument<string>("iwanna.exe", "Executable file path of fangame.");
        var modNames = new Argument<string[]>("mods", "Mod name to load");
        rootCommand.Add(executablePath);
        rootCommand.Add(modNames);
        rootCommand.SetHandler(ModAndRun, executablePath, modNames);
        rootCommand.Invoke(args);
    }

    static void ModAndRun(string executablePath, string[] modNames)
    {
        var sw = Stopwatch.StartNew();
        Modder modder = new Modder(executablePath, modNames);
        modder.ModAndRun();
        Console.WriteLine($"time cost: {sw.Elapsed.TotalSeconds}s");
    }
}
