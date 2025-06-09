using Fangame.ModLoader;

namespace GodMode;

public class GodModeMod : Mod
{
    public override void Load()
    {
        if (CommonData != null)
        {
            foreach (var script in CommonData.Scripts)
            {
                if (NameTable.Global.CheckIn(script.Name, "scrKillPlayer"))
                {
                    script.Source = "exit;\n" + script.Source;
                }
            }
        }
    }
}
