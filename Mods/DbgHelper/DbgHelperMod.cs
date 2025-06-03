using System.Diagnostics;
using System.Text;
using Fangame.ModLoader;
using UndertaleModLib;

namespace DbgHelper;

public class DbgHelperMod : Mod
{
    DbgHelperConfig Config = null!;

    public override void Load()
    {
        Config = GetConfig<DbgHelperConfig>();

        if (UndertaleData != null)
        {
            if (Config.DBGHELP)
            {
                CopyFileToRunningDirectory("DBGHELP.dll");
            }

            if (Config.SetSleepMargin)
            {
                foreach (var c in UndertaleData.Options.Constants)
                {
                    if (c.Name.Content == "@@SleepMargin")
                    {
                        c.Value = UndertaleData.Strings.MakeString(Config.SleepMargin.ToString());
                    }
                }
            }
        }
    }

    public override void ModExecutable()
    {
        if (ExecutableEngine == ExecutableEngine.GameMaker8)
        {
            StringBuilder args = new StringBuilder("-s -nb");
            if (!Config.InputLagPatch)
                args.Append(" -ni");
            if (!Config.JoystickPatch)
                args.Append(" -nj");
            if (!Config.SchedulerPatch)
                args.Append(" -ns");
            if (!Config.MemoryPatch)
                args.Append(" -nm");
            if (!Config.DirectPlayPatch)
                args.Append(" -nd");
            args.Append($" \"{RunningExecutablePath}\"");
            Process? p = Process.Start(new ProcessStartInfo
            {
                FileName = Path.Combine(ModDirectory, "gm8x_fix.exe"),
                Arguments = args.ToString(),
                UseShellExecute = false,
                CreateNoWindow = true,
            });
            p?.WaitForExit();
        }
    }
}
