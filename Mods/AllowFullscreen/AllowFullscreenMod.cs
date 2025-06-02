using Fangame.ModLoader;

namespace AllowFullscreen;

public class AllowFullscreenMod : Mod
{
    public override void Load()
    {
        if (GM8Data != null)
        {
            GM8Data.Settings.F4FullscreenToggle = true;
        }
    }
}
