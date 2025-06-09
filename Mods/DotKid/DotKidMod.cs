using Fangame.ModLoader;
using Fangame.ModLoader.Common;
using UndertaleModLib.Models;

namespace DotKid;

public class DotKidMod : Mod
{
    public override void Load()
    {
        if (CommonData != null)
        {
            var config = GetConfig<DotKidConfig>();

            foreach (var sprite in CommonData.Sprites)
            {
                if (NameTable.Global.CheckIn(sprite.Name, "sprPlayerMask"))
                {
                    sprite.ReplaceImages(Path.Combine(ModDirectory, "sprDotkidMask.png"), 1);
                    sprite.OriginX = 16;
                    sprite.OriginY = 16;
                }
                if (NameTable.Global.CheckIn(sprite.Name, "sprPlayerIdle", "sprPlayerRunning", "sprPlayerJump", "sprPlayerFall", "sprPlayerSliding"))
                {
                    sprite.ReplaceImages(Path.Combine(ModDirectory, "sprEmpty32x32.png"), 1);
                    sprite.OriginX = 16;
                    sprite.OriginY = 16;
                }
            }

            string code = """
                draw_sprite_ext(mask_index, image_index, x, y, image_xscale, image_yscale, image_angle, image_blend, image_alpha);
                """;

            if (config.ShowOutline)
            {
                code += """
                    draw_sprite_ext(sprDotkidModCircle, image_index, x, y, image_xscale, image_yscale, image_angle, image_blend, image_alpha);
                    """;

                var circle = CommonData.Sprites.CreateNew();
                circle.Name = "sprDotkidModCircle";
                circle.ReplaceImages(Path.Combine(ModDirectory, "sprDotkidCircle.png"), 1);
                circle.OriginX = 65;
                circle.OriginY = 64;
            }

            foreach (var obj in CommonData.Objects)
            {
                if (NameTable.Global.CheckIn(obj.Name, "objPlayer"))
                {
                    obj.EventAddCode(EventType.Draw, 0, code);
                }
            }
        }
    }
}
