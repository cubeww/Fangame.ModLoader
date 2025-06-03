using System.ComponentModel;
using Fangame.ModLoader;

namespace DbgHelper;

public class DbgHelperConfig : ModConfig
{
    [Category("Mod")]
    [ReadOnly(true)]
    [Description("Apply gm8x_fix and DBGHELP.dll patches to fix GameMaker lag issues.")]
    public string Version { get; set; } = "1.0.0";

    [Category("GM8")]
    public bool InputLagPatch { get; set; } = true;

    [Category("GM8")]
    public bool JoystickPatch { get; set; } = true;

    [Category("GM8")]
    public bool SchedulerPatch { get; set; } = true;

    [Category("GM8")]
    public bool MemoryPatch { get; set; } = true;

    [Category("GM8")]
    public bool DirectPlayPatch { get; set; } = true;

    [Category("GMS")]
    public bool DBGHELP { get; set; } = true;

    [Category("GMS")]
    public bool SetSleepMargin { get; set; } = true;

    [Category("GMS")]
    public int SleepMargin { get; set; } = 1;
}
