using Fangame.ModLoader;
using Fangame.ModLoader.Common;
using UndertaleModLib.Models;

namespace CrimsonKid;

public class CrimsonKidMod : Mod
{
    public override void Load()
    {
        var config = GetConfig<CrimsonKidConfig>();

        if (CommonData != null)
        {
            foreach (var sprite in CommonData.Sprites)
            {
                if (sprite.Name == "sprPlayerIdle")
                    sprite.ReplaceImages(Path.Combine(ModDirectory, "sprPlayerIdle.png"), 4);
                if (sprite.Name == "sprPlayerRunning")
                    sprite.ReplaceImages(Path.Combine(ModDirectory, "sprPlayerRunning.png"), 4);
                if (sprite.Name == "sprPlayerJump")
                    sprite.ReplaceImages(Path.Combine(ModDirectory, "sprPlayerJump.png"), 2);
                if (sprite.Name == "sprPlayerFall")
                    sprite.ReplaceImages(Path.Combine(ModDirectory, "sprPlayerFall.png"), 2);
                if (sprite.Name == "sprPlayerSliding")
                    sprite.ReplaceImages(Path.Combine(ModDirectory, "sprPlayerSliding.png"), 2);
                if (sprite.Name == "sprBow")
                {
                    sprite.ReplaceImages(Path.Combine(ModDirectory, "sprBow.png"), 4);
                    sprite.OriginX = 22;
                    sprite.OriginY = 23;
                }
            }

            CommonObject? bow = null;
            CommonObject? player = null;

            foreach (var obj in CommonData.Objects)
            {
                if (obj.Name == "player" || obj.Name == "objPlayer")
                    player = obj;
                if (obj.Name == "bow" || obj.Name == "objBow")
                    bow = obj;
            }

            if (bow != null)
            {
                if (player != null)
                    bow.Depth = player.Depth + 1;
                bow.EventAddCode(EventType.Create, 0, $"\nimage_speed = {config.BowImageSpeed};");
            }
        }
    }
}
