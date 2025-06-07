using System.ComponentModel;
using Fangame.ModLoader;

namespace DotKid;

public class DotKidConfig : ModConfig
{
    [Category("Sprite")]
    public bool ShowOutline { get; set; }
}
