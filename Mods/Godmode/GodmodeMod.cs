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
                if (script.Name is "scrKillPlayer" or "killPlayer" or "player_kill" or "kill_player")
                {
                    script.Source = "exit;\n" + script.Source;
                }
            }
        }
    }
}
