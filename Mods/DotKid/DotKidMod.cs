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
                switch (sprite.Name)
                {
                    case "maskPlayer" or "maskPlayer2" or "sprPlayerMask" or "sprPlayerMaskFlip" or "sprMaskPlayer" or "sprMaskPlayerFlip":
                        sprite.ReplaceImages(Path.Combine(ModDirectory, "sprDotkidMask.png"), 1);
                        sprite.OriginX = 16;
                        sprite.OriginY = 16;
                        break;
                    case "sprPlayerIdle" or "sprPlayerRun" or "sprPlayerRunning" or "sprPlayerJump" or "sprPlayerFall" or "sprPlayerSlide" or "sprPlayerSliding" or "sprBow":
                        sprite.ReplaceImages(Path.Combine(ModDirectory, "sprEmpty32x32.png"), 1);
                        sprite.OriginX = 16;
                        sprite.OriginY = 16;
                        break;
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
                switch (obj.Name)
                {
                    case "player" or "objPlayer" or "Player":
                        obj.EventAddCode(EventType.Draw, 0, code);
                        break;
                }
            }

        }
    }
}
