using System.Diagnostics;
using Fangame.ModLoader;
using UndertaleModLib;

namespace DbgHelper;

public class DbgHelperMod : Mod
{
    public override void Load()
    {
        if (UndertaleData != null)
        {
            CopyFileToRunningDirectory("DBGHELP.dll");
            foreach (var c in UndertaleData.Options.Constants)
            {
                if (c.Name.Content == "@@SleepMargin")
                {
                    c.Value = UndertaleData.Strings.MakeString("1");
                }
            }
        }
    }


    public override void ModExecutable()
    {
        if (ExecutableEngine == ExecutableEngine.GameMaker8)
        {
            Process? p = Process.Start(new ProcessStartInfo
            {
                FileName = Path.Combine(ModDirectory, "gm8x_fix.exe"),
                Arguments = $"-s -nb \"{RunningExecutablePath}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
            });
            p?.WaitForExit();
        }
    }
}
