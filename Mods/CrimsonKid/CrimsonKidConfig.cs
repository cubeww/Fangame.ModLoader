using System.ComponentModel;
using Fangame.ModLoader;

namespace CrimsonKid;

public class CrimsonKidConfig : ModConfig
{
    [Category("Sprite")]
    [Description("Image speed of the 'bow' sprite.")]
    public double BowImageSpeed { get; set; } = 0.2;
}
